from PIL import Image

original = Image.open("/Users/benny/Documents/GitHub/ARTexturePathFinding/NormalContrast/img/Topdown/Topdown_Ground/Normal_Ground080.png")

for res in range(1024, 3, -4):
    resized = original.resize((res, res), Image.BICUBIC)
    resized.save(f"Ground080Normal_{res}px.png")
