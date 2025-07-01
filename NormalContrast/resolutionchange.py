from PIL import Image
import os

# === CONFIGURATION ===
original_path = "/Users/benny/Documents/GitHub/ARTexturePathFinding/NormalContrast/img/Topdown/Topdown_Ground/Normal_Ground080.png"
save_folder = "Ground080Normals"
os.makedirs(save_folder, exist_ok=True)

# === LOAD ORIGINAL IMAGE ===
original = Image.open(original_path)

# === RESIZE LOOP ===
for res in range(1024, 3, -4):
    resized = original.resize((res, res), Image.BICUBIC)
    save_path = os.path.join(save_folder, f"Ground080Normal_{res}px.png")
    resized.save(save_path)

print(f"✅ All resized normal maps saved to: {save_folder}")
