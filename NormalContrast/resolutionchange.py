from PIL import Image

original = Image.open("/Users/benny/Documents/GitHub/ARTexturePathFinding/NormalContrast/img/Topdown/Topdown_Bricks/Normal_Bricks092.png")

for res in range(1024, 3, -4):
    resized = original.resize((res, res), Image.BICUBIC)
    resized.save(f"Bricks092Normal_{res}px.png")
