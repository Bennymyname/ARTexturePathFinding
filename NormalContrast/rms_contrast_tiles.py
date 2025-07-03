import os
import cv2
import numpy as np
import re
import matplotlib.pyplot as plt

# === CONFIGURATION ===
folders = {
    "Original": "/Users/benny/Documents/GitHub/ARTexturePathFinding/Bricks092TrilinearRepeat_cropped",
    "Equalisedrms": "/Users/benny/Documents/GitHub/ARTexturePathFinding/Bricks092TrilinearRepeat_equalised"


}

# === HELPER FUNCTIONS ===
def extract_resolution(filename):
    match = re.search(r'_(\d+)px', filename)
    return int(match.group(1)) if match else -1

def compute_crms_from_image(path):
    img = cv2.imread(path, cv2.IMREAD_UNCHANGED)
    if img is None:
        return None
    if img.dtype == np.uint16:
        img = (img / 256).astype(np.uint8)
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY).astype(np.float32)
    mean = gray.mean()
    std = gray.std()
    return std / mean if mean > 0 else 0

# === MAIN PROCESSING ===
results_by_label = {}

# Use filenames from the first folder as base
ref_folder = list(folders.values())[0]
all_filenames = sorted([f for f in os.listdir(ref_folder) if f.lower().endswith(('.png', '.jpg', '.jpeg'))], key=extract_resolution)

for label, folder_path in folders.items():
    crms_results = []
    for fname in all_filenames:
        res = extract_resolution(fname)
        full_path = os.path.join(folder_path, fname)
        if not os.path.exists(full_path):
            continue
        crms = compute_crms_from_image(full_path)
        if crms is not None:
            crms_results.append((res, crms))
    results_by_label[label] = crms_results

# === PLOT ===
fig, ax = plt.subplots(figsize=(12, 6))
colors = ['red', 'green', 'blue', 'orange', 'purple', 'brown', 'gray']
markers = ['x', 'o', '^', 's', 'd', '*', '+']

for idx, (label, results) in enumerate(results_by_label.items()):
    res_vals = [r[0] for r in results]
    crms_vals = [r[1] for r in results]
    ax.plot(res_vals, crms_vals, label=label, color=colors[idx % len(colors)], marker=markers[idx % len(markers)])

ax.set_title("Crms Comparison Across Folders")
ax.set_xlabel("Resolution (px)")
ax.set_ylabel("RMS Contrast (Crms)")
ax.grid(True)
ax.legend()
plt.tight_layout()
plt.show()
