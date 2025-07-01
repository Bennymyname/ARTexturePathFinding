import os
import cv2
import numpy as np

# === CONFIGURATION ===
source_dir = "2AFC/2AFC_Tester/Fabric065levels"
save_dir = "Fabric065TrilinearRepeat_cropped"
reference_image = "Fabric065Normal_1024px.png"  # <-- reference for cropping
gradient_threshold_ratio = 0.4
inward_margin_px = 10
preview_first = True

os.makedirs(save_dir, exist_ok=True)

def detect_crop_bounds(gray_img, threshold_ratio=0.4, margin=10):
    col_grad = np.abs(np.diff(np.mean(gray_img, axis=0)))
    threshold = threshold_ratio * np.max(col_grad)
    active_cols = np.where(col_grad > threshold)[0]

    if len(active_cols) < 2:
        raise ValueError("Edge detection failed. Try adjusting threshold ratio.")

    left = max(0, active_cols[0] + margin)
    right = min(gray_img.shape[1], active_cols[-1] - margin)
    return left, right

# === Load and detect bounds from reference image ===
ref_path = os.path.join(source_dir, reference_image)
ref_img = cv2.imread(ref_path)
if ref_img is None:
    raise FileNotFoundError(f"❌ Reference image not found: {reference_image}")

ref_gray = cv2.cvtColor(ref_img, cv2.COLOR_BGR2GRAY)
crop_bounds = detect_crop_bounds(ref_gray, threshold_ratio=gradient_threshold_ratio, margin=inward_margin_px)
left, right = crop_bounds

print(f"✅ Detected crop bounds from reference: columns {left} to {right}")

if preview_first:
    preview = ref_img[:, left:right]
    cv2.imshow("📷 Preview Cropped Reference", preview)
    print("🔍 Close preview to continue cropping all images...")
    cv2.waitKey(0)
    cv2.destroyAllWindows()

# === PROCESS ALL IMAGES ===
for fname in sorted(os.listdir(source_dir)):
    if not fname.lower().endswith(('.png', '.jpg', '.jpeg')):
        continue

    path = os.path.join(source_dir, fname)
    img = cv2.imread(path)
    if img is None:
        print(f"⚠️ Skipping unreadable: {fname}")
        continue

    cropped = img[:, left:right]
    save_path = os.path.join(save_dir, fname)
    cv2.imwrite(save_path, cropped)

print(f"\n✅ All cropped images saved to: {save_dir}")
