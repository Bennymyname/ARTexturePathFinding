#!/usr/bin/env python3
# normal_map_equalise_perpixel.py
"""
Nearest-neighbour upscale ➜ 1024×1024, then per-pixel slope‐magnitude matching
to a 1024-px reference normal map.

Example
-------
python /Users/benny/Documents/GitHub/ARTexturePathFinding/2AFC/2AFC_Tester/normal_map_equalise.py \
    --src  Bricks092Normals \
    --dst  Bricks092Normals_OUT \
    --ref  Bricks092Normal_1024px.png
"""

from pathlib import Path
import argparse, sys
import numpy as np
from PIL import Image


# ───────────────────────── helpers ─────────────────────────
def load_normal(p: Path):
    n = np.asarray(Image.open(p).convert("RGB"), np.float32) / 255.0
    x = n[..., 0] * 2.0 - 1.0
    y = n[..., 1] * 2.0 - 1.0
    z = np.clip(1.0 - x**2 - y**2, 0.0, 1.0) ** 0.5     # assume +Z hemisphere
    return x, y, z


def save_normal(p: Path, x, y, z):
    n = np.stack([(x + 1) / 2, (y + 1) / 2, (z + 1) / 2], -1)
    Image.fromarray((n * 255.0).astype(np.uint8)).save(p)


def upscale_nearest(img, size=(1024, 1024)):
    return img.resize(size, Image.NEAREST)


# ─────────────────────── main logic ───────────────────────
def process_folder(src, dst, ref_name, strength_cap=0.99):
    up_dir = dst / "upscaled"
    eq_dir = dst / "equalised_perpixel"
    up_dir.mkdir(parents=True, exist_ok=True)
    eq_dir.mkdir(parents=True, exist_ok=True)

    # load reference 1024-px map once
    xr, yr, zr = load_normal(src / ref_name)
    s_ref = np.sqrt(xr**2 + yr**2)        # per-pixel slope magnitude

    for fp in sorted(src.glob("*.png")):
        print(f"• {fp.name}")

        # 1) up-scale
        up = upscale_nearest(Image.open(fp))
        up_path = up_dir / fp.name
        up.save(up_path)

        # 2) per-pixel matching
        x, y, z = load_normal(up_path)
        s_cur = np.sqrt(x**2 + y**2) + 1e-8              # avoid ÷0
        g_pix = s_ref / s_cur                            # individual gain

        # cap so no vector exceeds |Nxy| = strength_cap
        g_cap = strength_cap / (s_cur * g_pix + 1e-8)
        g_pix = np.minimum(g_pix, g_cap)

        x *= g_pix
        y *= g_pix

        # If you wanted **full copy** instead of scaling, use:
        # x, y = xr.copy(), yr.copy()

        z = np.sqrt(np.clip(1.0 - x**2 - y**2, 0.0, 1.0))

        save_normal(eq_dir / fp.name, x, y, z)

    print("\n[✓] Done\n  up-scaled →", up_dir, "\n  per-pixel-equalised →", eq_dir)


# ────────────────────────── CLI ────────────────────────────
if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--src", required=True, help="folder with the PNG normal maps")
    ap.add_argument("--dst", required=True, help="output root folder")
    ap.add_argument("--ref", required=True, help="reference 1024-px filename (inside src)")
    args = ap.parse_args()

    process_folder(Path(args.src), Path(args.dst), args.ref)
