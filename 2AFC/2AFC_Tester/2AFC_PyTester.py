import pygame
import os
import random
import time
import pandas as pd
from datetime import datetime

# === SETTINGS ===
IMAGE_FOLDER = os.path.join(os.path.dirname(__file__), "Bricks092levels")
IMAGE_PREFIX = "Bricks092Normal_"
IMAGE_SUFFIX = "px.png"
FULL_RES = 1024
INITIAL_DISTRACTOR = 4
RES_STEP = 4
RESPONSE_TIMEOUT = 5  # seconds
SHOW_TIME = 0.5  # seconds
MAX_REVERSALS = 6

# === SETUP ===
pygame.init()
screen = pygame.display.set_mode((0, 0), pygame.FULLSCREEN)
screen_width, screen_height = screen.get_size()
font = pygame.font.SysFont(None, 60)
clock = pygame.time.Clock()

def load_image(res):
    path = os.path.join(IMAGE_FOLDER, f"{IMAGE_PREFIX}{res}{IMAGE_SUFFIX}")
    img = pygame.image.load(path)
    return pygame.transform.scale(img, (screen_height, screen_height))

def show_image(image, side):
    screen.fill((127, 127, 127))
    x = 0 if side == "left" else screen_width - screen_height
    screen.blit(image, (x, 0))
    pygame.display.flip()
    pygame.time.delay(int(SHOW_TIME * 1000))

def show_feedback(correct):
    color = (0, 255, 0) if correct else (255, 0, 0)
    screen.fill(color)
    pygame.display.flip()
    pygame.time.delay(500)

def get_user_choice():
    start_time = time.time()
    while True:
        elapsed = time.time() - start_time
        if elapsed > RESPONSE_TIMEOUT:
            return None, elapsed
        for event in pygame.event.get():
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_LEFT:
                    return "left", elapsed
                elif event.key == pygame.K_RIGHT:
                    return "right", elapsed
                elif event.key == pygame.K_q:
                    pygame.quit()
                    exit()
            elif event.type == pygame.QUIT:
                pygame.quit()
                exit()
        clock.tick(60)

def show_tutorial():
    instructions = [
        "Welcome to the 2AFC Test",
        "",
        "You will be shown two images one at a time (left, then right).",
        "One image is 1024px resolution, and the other is lower.",
        "",
        "Your task: Press ← or → to choose the side with the lower resolution image.",
        "You have 5 seconds to respond.",
        "",
        "Press SPACEBAR to begin, or Q to quit."
    ]
    screen.fill((0, 0, 0))
    for i, line in enumerate(instructions):
        text = font.render(line, True, (255, 255, 255))
        screen.blit(text, (100, 100 + i * 70))
    pygame.display.flip()

    waiting = True
    while waiting:
        for event in pygame.event.get():
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_SPACE:
                    waiting = False
                elif event.key == pygame.K_q:
                    pygame.quit()
                    exit()
            elif event.type == pygame.QUIT:
                pygame.quit()
                exit()
        clock.tick(60)

def main():
    show_tutorial()

    current_res = INITIAL_DISTRACTOR
    prev_correct = None
    reversal_count = 0
    trial_data = []
    trial_num = 1

    while reversal_count < MAX_REVERSALS and current_res < FULL_RES:
        current_res = max(4, min(1020, (current_res // 4) * 4))

        fullres_side = random.choice(["left", "right"])
        distractor_side = "right" if fullres_side == "left" else "left"

        full_img = load_image(FULL_RES)
        distractor_img = load_image(current_res)

        if fullres_side == "left":
            show_image(full_img, "left")
            show_image(distractor_img, "right")
        else:
            show_image(distractor_img, "left")
            show_image(full_img, "right")

        choice, response_time = get_user_choice()
        if choice is None:
            correct = False
            chosen = "none"
        else:
            chosen = choice
            correct = (choice == distractor_side)

        show_feedback(correct)

        left_res = FULL_RES if fullres_side == "left" else current_res
        right_res = FULL_RES if fullres_side == "right" else current_res
        trial_data.append({
            "Trial": trial_num,
            "Left_Image": f"{IMAGE_PREFIX}{left_res}{IMAGE_SUFFIX}",
            "Right_Image": f"{IMAGE_PREFIX}{right_res}{IMAGE_SUFFIX}",
            "Left_Res": left_res,
            "Right_Res": right_res,
            "Correct_Answer": distractor_side,
            "User_Choice": chosen,
            "Is_Correct": correct,
            "Response_Time_ms": int(response_time * 1000)
        })

        new_res = (current_res + FULL_RES) // 2 if correct else current_res - RES_STEP
        if prev_correct is not None and prev_correct != correct:
            reversal_count += 1
        prev_correct = correct
        current_res = new_res
        trial_num += 1

    # Save results
    df = pd.DataFrame(trial_data)
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    df.to_excel(f"results_{timestamp}.xlsx", index=False)

    # End screen
    screen.fill((0, 0, 0))
    end_text = font.render("Test Complete. Press ESC or Q to Exit.", True, (255, 255, 255))
    screen.blit(end_text, ((screen_width - end_text.get_width()) // 2, screen_height // 2))
    pygame.display.flip()

    waiting = True
    while waiting:
        for event in pygame.event.get():
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_ESCAPE or event.key == pygame.K_q:
                    waiting = False
            elif event.type == pygame.QUIT:
                waiting = False
        clock.tick(60)

    pygame.quit()

if __name__ == "__main__":
    main()
