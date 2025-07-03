import os
import cv2
import numpy as np
from pathlib import Path
from matplotlib import pyplot as plt

# === CONFIGURATION ===
src_folder = Path("/Users/benny/Documents/GitHub/ARTexturePathFinding/Bricks092TrilinearRepeat_cropped")
dst_folder = Path("/Users/benny/Documents/GitHub/ARTexturePathFinding/Bricks092TrilinearRepeat_equalised")
reference_crms = 0.0335  # Target Crms
dst_folder.mkdir(parents=True, exist_ok=True)

def compute_luminance(rgb):
    """Convert RGB to luminance using BT.709 coefficients (no gamma)."""
    rgb = rgb.astype(np.float32) / 255.0
    return 0.2126 * rgb[..., 2] + 0.7152 * rgb[..., 1] + 0.0722 * rgb[..., 0]

def compute_crms(luminance):
    mu = luminance.mean()
    sigma = luminance.std()
    return sigma / (mu + 1e-8)

def scale_image_to_crms(img, target_crms):
    luminance = compute_luminance(img)
    mu = luminance.mean()
    sigma = luminance.std()
    crms_current = sigma / (mu + 1e-8)
    gain = target_crms / (crms_current + 1e-8)
    img_float = img.astype(np.float32)
    img_scaled = (img_float - mu * 255.0) * gain + mu * 255.0
    return np.clip(img_scaled, 0, 255).astype(np.uint8), gain, crms_current

# === Batch Processing ===
log_data = []

for fname in sorted(os.listdir(src_folder)):
    if not fname.lower().endswith(('.png', '.jpg', '.jpeg')):
        continue

    src_path = src_folder / fname
    dst_path = dst_folder / fname

    img = cv2.imread(str(src_path))
    if img is None:
        continue

    img_eq, gain, crms_before = scale_image_to_crms(img, reference_crms)
    cv2.imwrite(str(dst_path), img_eq)

    crms_after = compute_crms(compute_luminance(img_eq))
    log_data.append((fname, crms_before, crms_after, gain))

# === Plot Results ===
filenames, crms_before_list, crms_after_list, gain_list = zip(*log_data)

plt.figure(figsize=(12, 6))
plt.plot(crms_before_list, label="Before Equalization", marker='x', color='red')
plt.plot(crms_after_list, label="After Equalization", marker='o', color='green')
plt.axhline(reference_crms, linestyle='--', color='gray', label="Target Crms")
plt.title("Crms Before and After Equalization")
plt.xlabel("Image Index")
plt.ylabel("Crms")
plt.legend()
plt.grid(True)
plt.tight_layout()
plt.show()
