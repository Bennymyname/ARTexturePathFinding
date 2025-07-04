import cv2
import numpy as np
import matplotlib.pyplot as plt
import os
import math
from scipy.stats import skew, kurtosis

# === CONFIGURATION ===
process_all_folders = False
target_folder = ["Topdown_Bricks"]
images_per_page = 4  # Max images per figure
cols_per_page = 4    # Layout: 4 columns per page

base_dir = "NormalContrast/img"
valid_exts = ('.png', '.jpg', '.jpeg', '.bmp', '.tiff')

# === COLLECT IMAGES AND METRICS ===
results = []

if process_all_folders:
    folders_to_process = [
        f for f in os.listdir(base_dir)
        if os.path.isdir(os.path.join(base_dir, f)) and f.startswith("Topdown_")
    ]
else:
    folders_to_process = target_folder

for folder in folders_to_process:
    folder_path = os.path.join(base_dir, folder)
    for fname in sorted(os.listdir(folder_path)):
        if fname.startswith("Topdown_") and fname.lower().endswith(valid_exts):
            path = os.path.join(folder_path, fname)
            img_bgr = cv2.imread(path, cv2.IMREAD_UNCHANGED)
            if img_bgr is None:
                print(f"⚠️ Could not load: {path}")
                continue
            if img_bgr.dtype == np.uint16:
                img_bgr = (img_bgr / 256).astype(np.uint8)

            gray = cv2.cvtColor(img_bgr, cv2.COLOR_BGR2GRAY).astype(np.float32)
            mean = gray.mean()
            std = gray.std()
            rms = std / mean if mean > 0 else 0
            var = np.var(gray)
            sk = skew(gray.ravel())
            kurt_val = kurtosis(gray.ravel())

            results.append({
                "folder": folder,
                "filename": fname,
                "rms": rms,
                "var": var,
                "skew": sk,
                "kurt": kurt_val,
                "gray": gray,
                "img": cv2.cvtColor(img_bgr, cv2.COLOR_BGR2RGB)
            })

if len(results) == 0:
    raise ValueError("No valid images found.")

# === PAGINATED FIGURE GENERATION ===
total = len(results)
pages = math.ceil(total / images_per_page)

for page in range(pages):
    start = page * images_per_page
    end = min(start + images_per_page, total)
    batch = results[start:end]
    n = len(batch)

    cols = min(cols_per_page, n)
    rows = math.ceil(n / cols)

    fig, axes = plt.subplots(2 * rows, cols, figsize=(5 * cols, 6 * rows),
                             gridspec_kw={'height_ratios': [2, 3] * rows})
    axes = np.array(axes).reshape(2 * rows, cols)

    max_y = max(np.histogram(r["gray"], bins=256, range=(0, 255))[0].max() for r in batch)

    for i, r in enumerate(batch):
        row = i // cols
        col = i % cols
        ax_h = axes[2 * row, col]
        ax_img = axes[2 * row + 1, col]

        hist, bins = np.histogram(r["gray"], bins=256, range=(0, 255))
        ax_h.plot(bins[:-1], hist, color='darkblue', linewidth=1.5)
        ax_h.set_ylim(0, max_y * 1.05)
        ax_h.set_title(f"{r['folder']}/{r['filename']}", fontsize=10)
        ax_h.set_xlabel("Luminance (0–255)")
        ax_h.set_ylabel("Count")
        ax_h.grid(True)
        ax_h.text(
            0.02, 0.97,
            f"RMS: {r['rms']:.4f}\nVar: {r['var']:.2f}\nSkew: {r['skew']:.2f}\nKurt: {r['kurt']:.2f}",
            transform=ax_h.transAxes,
            fontsize=9,
            verticalalignment='top',
            bbox=dict(boxstyle="round,pad=0.3", fc="lightyellow", ec="black", alpha=0.8)
        )

        ax_img.imshow(r["img"])
        ax_img.set_title("Original Image", fontsize=10)
        ax_img.axis("off")

    # Turn off any unused axes
    total_cells = 2 * rows * cols
    for i in range(n, cols * rows):
        r_idx = 2 * (i // cols)
        c_idx = i % cols
        axes[r_idx, c_idx].axis("off")
        axes[r_idx + 1, c_idx].axis("off")

    plt.tight_layout()
    filename = f"histogram_page_{page + 1}.png"
    plt.savefig(filename, dpi=300)
    print(f"✅ Saved: {filename}")
    plt.close()
