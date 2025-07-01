import os
import cv2
import numpy as np
import re
import matplotlib.pyplot as plt
import mplcursors  # For hover interaction

# === CONFIGURATION ===
image_folder = "Fabric065TrilinearRepeat_cropped"

# === HELPER FUNCTION ===
def extract_resolution(filename):
    match = re.search(r'_(\d+)px', filename)
    return int(match.group(1)) if match else -1

# === MAIN SCRIPT ===
results = []

for filename in sorted(os.listdir(image_folder), key=extract_resolution):
    if filename.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp')):
        path = os.path.join(image_folder, filename)
        img = cv2.imread(path, cv2.IMREAD_UNCHANGED)
        if img is None:
            print(f"⚠️ Skipping unreadable: {filename}")
            continue
        if img.dtype == np.uint16:
            img = (img / 256).astype(np.uint8)
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY).astype(np.float32)
        mean = gray.mean()
        std = gray.std()
        rms = std / mean if mean > 0 else 0
        resolution = extract_resolution(filename)
        results.append((resolution, rms))

# === SORT AND DISPLAY ===
results.sort()
print(f"\n{'Filename':<15}  {'Resolution':<12}  {'RMS_Contrast'}")
print("-" * 45)
for res, rms in results:
    print(f"{f'{res}px':<15}  {res:<12}  {rms:.6f}")

# === PLOT ===
resolutions = [r[0] for r in results]
rms_values = [r[1] for r in results]

fig, ax = plt.subplots(figsize=(10, 5))
line, = ax.plot(resolutions, rms_values, marker='o', linestyle='-', color='blue')

# Hover tooltip
cursor = mplcursors.cursor(line, hover=True)
cursor.connect("add", lambda sel: sel.annotation.set_text(
    f"Resolution: {resolutions[sel.index]}px\nCrms: {rms_values[sel.index]:.4f}"))

ax.set_title("RMS Contrast vs Resolution (Hover to Inspect)")
ax.set_xlabel("Resolution (px)")
ax.set_ylabel("RMS Contrast (Crms)")
ax.grid(True)
plt.tight_layout()
plt.show()
