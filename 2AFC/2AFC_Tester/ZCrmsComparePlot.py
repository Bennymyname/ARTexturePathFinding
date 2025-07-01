import os
import cv2
import numpy as np
import re
import matplotlib.pyplot as plt

# === CONFIGURATION ===
normal_map_folder = "Ground080Normals"
crms_image_folder = "Ground080TrilinearRepeat_cropped"

# === HELPER FUNCTIONS ===
def extract_resolution(filename):
    match = re.search(r'_(\d+)px', filename)
    return int(match.group(1)) if match else -1

def decode_normal_components(normal_img):
    rgb = normal_img.astype(np.float32) / 255.0
    x = rgb[..., 0] * 2.0 - 1.0
    y = rgb[..., 1] * 2.0 - 1.0
    z = rgb[..., 2] * 2.0 - 1.0
    z = np.clip(z, -1.0, 1.0)
    return x, y, z

# === STEP 1: Extract normal map metrics ===
z_mean_list, z_std_list, xy_strength_list, angle_deg_list, resolutions = [], [], [], [], []

for fname in sorted(os.listdir(normal_map_folder), key=extract_resolution):
    if not fname.lower().endswith(('.png', '.jpg', '.jpeg')):
        continue
    path = os.path.join(normal_map_folder, fname)
    img = cv2.imread(path, cv2.IMREAD_COLOR)
    if img is None:
        continue

    x, y, z = decode_normal_components(img)
    strength = np.sqrt(x**2 + y**2)
    angle_rad = np.arccos(z)
    angle_deg = np.degrees(angle_rad)

    res = extract_resolution(fname)
    resolutions.append(res)
    z_mean_list.append(np.mean(z))
    z_std_list.append(np.std(z))
    xy_strength_list.append(np.mean(strength))
    angle_deg_list.append(np.mean(angle_deg))

# === STEP 2: Crms from user-view images ===
crms_values = []
for fname in sorted(os.listdir(crms_image_folder), key=extract_resolution):
    if not fname.lower().endswith(('.png', '.jpg', '.jpeg')):
        continue
    path = os.path.join(crms_image_folder, fname)
    img = cv2.imread(path, cv2.IMREAD_UNCHANGED)
    if img is None:
        continue
    if img.dtype == np.uint16:
        img = (img / 256).astype(np.uint8)
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY).astype(np.float32)
    mean, std = gray.mean(), gray.std()
    rms = std / mean if mean > 0 else 0
    crms_values.append(rms)

# === STEP 3: Plot with 5 Y-axes ===
fig, ax1 = plt.subplots(figsize=(13, 7))
ax1.set_xlabel("Resolution (px)")

# Y-axis 1: Mean Z (left)
ln1 = ax1.plot(resolutions, z_mean_list, label="Mean Z", color='blue', marker='o')
ax1.set_ylabel("Mean Z", color='blue')
ax1.tick_params(axis='y', labelcolor='blue')

# Y-axis 2: Crms (right)
ax2 = ax1.twinx()
ln2 = ax2.plot(resolutions, crms_values, label="Crms", color='red', marker='x')
ax2.set_ylabel("Crms", color='red')
ax2.tick_params(axis='y', labelcolor='red')

# Y-axis 3: Std Z (right)
ax3 = ax1.twinx()
ax3.spines.right.set_position(("axes", 1.1))
ln3 = ax3.plot(resolutions, z_std_list, label="Std Z", color='green', linestyle='--', marker='^')
ax3.set_ylabel("Std Z", color='green')
ax3.tick_params(axis='y', labelcolor='green')

# Y-axis 4: XY Strength (right)
ax4 = ax1.twinx()
ax4.spines.right.set_position(("axes", 1.2))
ln4 = ax4.plot(resolutions, xy_strength_list, label="XY Strength", color='orange', linestyle='-.', marker='s')
ax4.set_ylabel("XY Strength", color='orange')
ax4.tick_params(axis='y', labelcolor='orange')

# Y-axis 5: Angle Deviation (°) (right)
ax5 = ax1.twinx()
ax5.spines.right.set_position(("axes", 1.3))
ln5 = ax5.plot(resolutions, angle_deg_list, label="Angle (°)", color='purple', linestyle=':', marker='d')
ax5.set_ylabel("Angle Deviation (°)", color='purple')
ax5.tick_params(axis='y', labelcolor='purple')

# Combine all lines for legend
lines = ln1 + ln2 + ln3 + ln4 + ln5
labels = [l.get_label() for l in lines]
ax1.legend(lines, labels, loc='upper left')

plt.title("Normal Map Metrics vs Crms across Resolution")
plt.grid(True)
plt.tight_layout()
plt.show()
