import os
import cv2
import numpy as np
import matplotlib.pyplot as plt
from scipy.stats import skew, kurtosis

# === CONFIGURATION ===
process_all_folders = True
target_folder = ["Topdown_Bricks", "Topdown_PavingStones"]
base_dir = "NormalContrast/img"
sort_by = "Skewness"  # Options: 'Kurtosis', 'RMS_Contrast', 'Variance', 'Skewness'

# === LOAD IMAGES & CALCULATE STATS ===
valid_exts = ('.png', '.jpg', '.jpeg', '.bmp', '.tiff')
folders_to_process = (
    [f for f in os.listdir(base_dir)
     if os.path.isdir(os.path.join(base_dir, f)) and f.startswith("Topdown_")]
    if process_all_folders else target_folder
)

results = []
for folder in folders_to_process:
    folder_path = os.path.join(base_dir, folder)
    for fname in sorted(os.listdir(folder_path)):
        if fname.startswith("Topdown_") and fname.lower().endswith(valid_exts):
            path = os.path.join(folder_path, fname)
            img_bgr = cv2.imread(path, cv2.IMREAD_UNCHANGED)
            if img_bgr is None:
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
                "Folder": folder,
                "Filename": fname,
                "RMS_Contrast": rms,
                "Variance": var,
                "Skewness": sk,
                "Kurtosis": kurt_val,
                "RGB": cv2.cvtColor(img_bgr, cv2.COLOR_BGR2RGB)
            })

# === SORTING & DISPLAYING ===
results.sort(key=lambda x: x[sort_by], reverse=True)
top5 = results[:5]
bottom5 = results[-5:]

# === PRINT FULL TABLE TO TERMINAL ===
print(f"\nSorted by {sort_by}:\n")
print(f"{'Index':<5} {'Filename':<30} {'RMS':>8} {'Var':>10} {'Skew':>10} {'Kurtosis':>10}")
print("-" * 75)
for i, r in enumerate(results):
    print(f"{i+1:<5} {r['Filename']:<30} {r['RMS_Contrast']:.4f} {r['Variance']:>10.2f} "
          f"{r['Skewness']:>10.2f} {r['Kurtosis']:>10.2f}")

# === PLOT IMAGES (TOP 5 + BOTTOM 5) ===
fig, axes = plt.subplots(2, 5, figsize=(20, 5))
combined = top5 + bottom5

for ax, r in zip(axes.flatten(), combined):
    ax.imshow(r["RGB"])
    ax.set_title(f"{r['Filename']}\n{sort_by}: {r[sort_by]:.2f}", fontsize=9)
    ax.axis("off")

plt.suptitle(f"Top 5 and Bottom 5 Images by {sort_by}", fontsize=14)
plt.tight_layout(rect=[0, 0, 1, 0.92])
plt.show()
