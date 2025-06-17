import os
import cv2
import numpy as np
import matplotlib.pyplot as plt

base_dir = "img" 

# Filenames and labels
images_info = [
    ("Normal_Fabric061.png",  "Normal Fabric 061"),
    ("Normal_Fabric065.png",  "Normal Fabric 065"),
    ("Topdown_Fabric061.png", "Top-down Fabric 061"),
    ("Topdown_Fabric065.png", "Top-down Fabric 065"),
]

# 2 by 2 layout
fig, axes = plt.subplots(2, 2, figsize=(16, 8))

results = {}

for ax, (fname, label) in zip(axes.flatten(), images_info):
    path = os.path.join(base_dir, fname)
    bgr = cv2.imread(path, cv2.IMREAD_UNCHANGED)
    if bgr is None:
        raise FileNotFoundError(f"Image not found: {path}")

    # Handle 16-bit images: convert to 8-bit Because normal maps are in 16 bit can not displayed in matplotlib
    if bgr.dtype == np.uint16:
        bgr = (bgr / 256).astype(np.uint8)

    # Convert to RGB
    rgb = cv2.cvtColor(bgr, cv2.COLOR_BGR2RGB)
    ax.imshow(rgb)
    ax.set_title(label, fontsize=11)
    ax.axis("off")

    # Convert to grayscale luminance
    gray = cv2.cvtColor(bgr, cv2.COLOR_BGR2GRAY).astype(np.float32) / 255.0

    # Calculating mean and standard deviation
    mu = gray.mean()
    sigma = gray.std()

    # Standard RMS contrast (σ)
    rms_std = sigma

    # Normalized RMS contrast (σ² / μ)  Crms equation from Prof.C
    norm_rms = (sigma ** 2) / mu if mu > 0 else 0

    # Save results
    results[label] = (rms_std, norm_rms)

    # Displaying
    ax.text(
        0.02, 0.90,
        f"RMS: {rms_std:.4f}\nNorm RMS: {norm_rms:.4f}",
        transform=ax.transAxes,
        fontsize=10, color="white", weight="bold",
        bbox=dict(boxstyle="round,pad=0.25", fc="black", alpha=0.6)
    )

plt.tight_layout()
plt.show()

# Print summary
print("\nRMS and Normalized RMS contrast values:")
print("-----------------------------------------")
for name, (rms_val, norm_val) in results.items():
    print(f"{name:<22}  RMS: {rms_val:.4f}    Norm RMS: {norm_val:.4f}")
