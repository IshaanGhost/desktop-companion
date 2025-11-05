#!/usr/bin/env python3
"""
Generate a 16-frame pixel art cat running animation sprite sheet.
Each frame: 32x32 pixels
Full sheet: 512x32 pixels (16 frames × 32 pixels)
Style: Retro 16-bit pixel art with 4-color palette
Format: PNG with transparent background
"""

from PIL import Image, ImageDraw
import math

# 4-color palette (retro 16-bit style)
PALETTE = {
    'bg': (0, 0, 0, 0),      # Transparent
    'dark': (64, 48, 96),    # Dark purple-gray (body shadow)
    'main': (128, 96, 160),  # Medium purple-gray (main body)
    'light': (192, 160, 192), # Light purple-gray (highlights)
    'accent': (255, 224, 192) # Light cream (belly/face)
}

def draw_cat_frame(draw, frame_num, frame_width=32, frame_height=32):
    """Draw a single frame of the running cat animation."""
    # Clear frame
    # (transparent background is default)
    
    # Animation parameters
    cycle = 16
    t = frame_num / cycle * 2 * math.pi
    
    # Body bounce (vertical position oscillates)
    body_y_offset = int(3 * math.sin(t * 2))
    
    # Leg positions (alternating)
    front_leg_forward = int(4 * math.sin(t))
    back_leg_forward = int(4 * math.sin(t + math.pi))
    
    # Tail wag
    tail_angle = int(8 * math.sin(t * 1.5))
    
    center_x = frame_width // 2
    center_y = frame_height // 2 + body_y_offset
    
    # Draw cat body (ellipse)
    body_width, body_height = 18, 12
    body_left = center_x - body_width // 2
    body_top = center_y - body_height // 2 + 2
    
    # Main body
    draw.ellipse([body_left, body_top, body_left + body_width, body_top + body_height], 
                 fill=PALETTE['main'], outline=PALETTE['dark'])
    
    # Belly highlight
    draw.ellipse([body_left + 2, body_top + 3, body_left + body_width - 2, body_top + body_height - 2], 
                 fill=PALETTE['accent'])
    
    # Head (circle, slightly above body)
    head_radius = 6
    head_x = center_x
    head_y = center_y - 4
    draw.ellipse([head_x - head_radius, head_y - head_radius, 
                  head_x + head_radius, head_y + head_radius], 
                 fill=PALETTE['main'], outline=PALETTE['dark'])
    
    # Face highlight
    draw.ellipse([head_x - head_radius + 2, head_y - head_radius + 2, 
                  head_x + head_radius - 2, head_y + head_radius - 2], 
                 fill=PALETTE['accent'])
    
    # Ears (two triangles)
    ear_size = 4
    # Left ear
    draw.polygon([(head_x - 4, head_y - head_radius), 
                  (head_x - 2, head_y - head_radius - ear_size), 
                  (head_x, head_y - head_radius)], 
                 fill=PALETTE['dark'], outline=PALETTE['dark'])
    # Right ear
    draw.polygon([(head_x + 4, head_y - head_radius), 
                  (head_x + 2, head_y - head_radius - ear_size), 
                  (head_x, head_y - head_radius)], 
                 fill=PALETTE['dark'], outline=PALETTE['dark'])
    
    # Eyes (two small dots)
    eye_size = 2
    draw.ellipse([head_x - 3, head_y - 1, head_x - 3 + eye_size, head_y - 1 + eye_size], 
                 fill=PALETTE['dark'])
    draw.ellipse([head_x + 1, head_y - 1, head_x + 1 + eye_size, head_y - 1 + eye_size], 
                 fill=PALETTE['dark'])
    
    # Nose (small triangle)
    draw.polygon([(head_x, head_y + 1), (head_x - 1, head_y + 3), (head_x + 1, head_y + 3)], 
                 fill=PALETTE['dark'])
    
    # Front legs (two)
    leg_width, leg_height = 3, 6
    # Left front leg
    leg_x = center_x - 6 + front_leg_forward
    leg_y = center_y + 4
    draw.rectangle([leg_x, leg_y, leg_x + leg_width, leg_y + leg_height], 
                   fill=PALETTE['dark'], outline=PALETTE['dark'])
    # Right front leg
    leg_x = center_x + 3 + front_leg_forward
    draw.rectangle([leg_x, leg_y, leg_x + leg_width, leg_y + leg_height], 
                   fill=PALETTE['dark'], outline=PALETTE['dark'])
    
    # Back legs (two)
    # Left back leg
    leg_x = center_x - 7 + back_leg_forward
    leg_y = center_y + 5
    draw.rectangle([leg_x, leg_y, leg_x + leg_width, leg_y + leg_height], 
                   fill=PALETTE['dark'], outline=PALETTE['dark'])
    # Right back leg
    leg_x = center_x + 4 + back_leg_forward
    draw.rectangle([leg_x, leg_y, leg_x + leg_width, leg_y + leg_height], 
                   fill=PALETTE['dark'], outline=PALETTE['dark'])
    
    # Tail (curved)
    tail_x = center_x + body_width // 2
    tail_y = center_y
    tail_end_x = tail_x + 6 + tail_angle
    tail_end_y = tail_y - 4
    # Draw tail as a series of small rectangles
    for i in range(4):
        t_val = i / 3
        x = int(tail_x + (tail_end_x - tail_x) * t_val)
        y = int(tail_y + (tail_end_y - tail_y) * t_val)
        draw.rectangle([x, y, x + 2, y + 3], fill=PALETTE['main'], outline=PALETTE['dark'])

def create_sprite_sheet():
    """Create the full 16-frame sprite sheet."""
    frame_width = 32
    frame_height = 32
    num_frames = 16
    sheet_width = frame_width * num_frames
    sheet_height = frame_height
    
    # Create image with transparent background
    img = Image.new('RGBA', (sheet_width, sheet_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Draw each frame
    for frame in range(num_frames):
        # Create a temporary image for this frame
        frame_img = Image.new('RGBA', (frame_width, frame_height), (0, 0, 0, 0))
        frame_draw = ImageDraw.Draw(frame_img)
        
        # Draw the cat in this frame
        draw_cat_frame(frame_draw, frame, frame_width, frame_height)
        
        # Paste frame into sprite sheet
        x_offset = frame * frame_width
        img.paste(frame_img, (x_offset, 0), frame_img)
    
    return img

if __name__ == '__main__':
    print("Generating 16-frame cat running animation sprite sheet...")
    sprite_sheet = create_sprite_sheet()
    output_file = 'cat_running_spritesheet.png'
    sprite_sheet.save(output_file, 'PNG')
    print(f"Sprite sheet saved as '{output_file}'")
    print(f"  Size: {sprite_sheet.width}x{sprite_sheet.height} pixels")
    print(f"  Frames: 16 (each 32x32 pixels)")
    print(f"  Format: PNG with transparent background")

