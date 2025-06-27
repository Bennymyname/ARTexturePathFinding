import os
import cv2
import numpy as np

# === CONFIGURATION ===
source_dir = "2AFC/2AFC_Tester/Ground080TrilinearRepeat"
save_dir = "Ground080TrilinearRepeat_cropped"
gradient_threshold_ratio = 0.4     # Stronger sensitivity
inward_margin_px = 10              # Additional trim from both sides
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

# === PROCESSING LOOP ===
crop_bounds = None
for i, fname in enumerate(sorted(os.listdir(source_dir))):
    if not fname.lower().endswith(('.png', '.jpg', '.jpeg')):
        continue

    path = os.path.join(source_dir, fname)
    img = cv2.imread(path)
    if img is None:
        print(f"⚠️ Skipping unreadable: {fname}")
        continue

    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    if crop_bounds is None:
        try:
            crop_bounds = detect_crop_bounds(gray, threshold_ratio=gradient_threshold_ratio, margin=inward_margin_px)
        except ValueError as e:
            print(f"❌ Failed on {fname}: {e}")
            break

        print(f"✅ Detected bounds: columns {crop_bounds[0]} to {crop_bounds[1]}")
        if preview_first:
            preview = img[:, crop_bounds[0]:crop_bounds[1]]
            cv2.imshow("Preview Cropped Image", preview)
            print("🔍 Close preview to continue...")
            cv2.waitKey(0)
            cv2.destroyAllWindows()

    # Apply cropping
    left, right = crop_bounds
    cropped = img[:, left:right]
    save_path = os.path.join(save_dir, fname)
    cv2.imwrite(save_path, cropped)

print(f"\n✅ All cropped images saved to: {save_dir}")
