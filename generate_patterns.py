"""
Pattern Image Generator for Quest VR Tracing App
Generates tracing patterns with varying difficulty levels:
- Line width: 5 levels (10px to 50px, wider = easier)
- Complexity: Straight lines → Simple shapes → Complex shapes → Curves → Complex curves
"""

from PIL import Image, ImageDraw
import os
import math

# Configuration
SCALE_SIZES = [
    (512, 512),
    (960, 540),
    (1280, 720),
    (1920, 1080),
    (2560, 1440),
    (3840, 2160),
    (4096, 2304),
    (5120, 2880),
    (5760, 3240),
    (7680, 4320)
]
IMAGE_SIZE = 1024  # Default, will be overridden per scale
OUTPUT_DIR = "TracingApp/Assets/Resources/Patterns"
BACKGROUND_COLOR = (0, 0, 0, 0)  # Transparent
LINE_COLOR = (255, 255, 255, 255)  # White

# Line width levels (pixels)
LINE_WIDTHS = {
    1: 10,   # Very thin - hardest
    2: 20,   # Thin
    3: 30,   # Medium
    4: 40,   # Thick
    5: 50    # Very thick - easiest
}

def create_image(width, height):
    """Create a new transparent image with given size"""
    return Image.new('RGBA', (width, height), BACKGROUND_COLOR)

def get_center(width, height):
    """Get center point of image"""
    return width // 2, height // 2

def get_margin(width, height):
    """Get margin from edges"""
    return min(width, height) // 8

def draw_horizontal_line(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h)
    y = img_h // 2
    draw.line([(margin, y), (img_w - margin, y)], fill=LINE_COLOR, width=width)

def draw_vertical_line(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h)
    x = img_w // 2
    draw.line([(x, margin), (x, img_h - margin)], fill=LINE_COLOR, width=width)

def draw_diagonal_line(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h)
    draw.line([(margin, margin), (img_w - margin, img_h - margin)], fill=LINE_COLOR, width=width)

def draw_zigzag(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h)
    points = []
    num_points = 5
    for i in range(num_points):
        x = margin + (img_w - 2 * margin) * i / (num_points - 1)
        y = margin if i % 2 == 0 else img_h - margin
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="curve")

def draw_square(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    draw.rectangle([margin, margin, img_w - margin, img_h - margin], outline=LINE_COLOR, width=width)

def draw_triangle(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    cx, cy = get_center(img_w, img_h)
    size = min(img_w, img_h) - 2 * margin
    height = size * math.sqrt(3) / 2
    points = [
        (cx, margin),
        (margin, img_h - margin),
        (img_w - margin, img_h - margin),
        (cx, margin)
    ]
    draw.line(points, fill=LINE_COLOR, width=width, joint="miter")

def draw_rectangle(draw, width, img_w, img_h):
    margin_x = get_margin(img_w, img_h) * 2
    margin_y = get_margin(img_w, img_h) * 1.2
    draw.rectangle([margin_x, margin_y, img_w - margin_x, img_h - margin_y], outline=LINE_COLOR, width=width)

def draw_cross(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.3
    cx, cy = get_center(img_w, img_h)

    # Continuous, closed "plus" outline (single-stroke loop)
    outer = (min(img_w, img_h) - 2 * margin) / 2
    arm_half = outer * 0.35

    left = cx - outer
    right = cx + outer
    top = cy - outer
    bottom = cy + outer

    inner_l = cx - arm_half
    inner_r = cx + arm_half
    inner_t = cy - arm_half
    inner_b = cy + arm_half

    points = [
        (inner_l, top),
        (inner_r, top),
        (inner_r, inner_t),
        (right, inner_t),
        (right, inner_b),
        (inner_r, inner_b),
        (inner_r, bottom),
        (inner_l, bottom),
        (inner_l, inner_b),
        (left, inner_b),
        (left, inner_t),
        (inner_l, inner_t),
        (inner_l, top)
    ]
    draw.line(points, fill=LINE_COLOR, width=width, joint="miter")

def draw_pentagon(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    cx, cy = get_center(img_w, img_h)
    radius = (min(img_w, img_h) - 2 * margin) / 2
    points = []
    for i in range(6):
        angle = math.radians(i * 72 - 90)
        x = cx + radius * math.cos(angle)
        y = cy + radius * math.sin(angle)
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="miter")

def draw_hexagon(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    cx, cy = get_center(img_w, img_h)
    radius = (min(img_w, img_h) - 2 * margin) / 2
    points = []
    for i in range(7):
        angle = math.radians(i * 60)
        x = cx + radius * math.cos(angle)
        y = cy + radius * math.sin(angle)
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="miter")

def draw_star(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    cx, cy = get_center(img_w, img_h)
    outer_radius = (min(img_w, img_h) - 2 * margin) / 2
    inner_radius = outer_radius * 0.4
    points = []
    for i in range(11):
        angle = math.radians(i * 36 - 90)
        radius = outer_radius if i % 2 == 0 else inner_radius
        x = cx + radius * math.cos(angle)
        y = cy + radius * math.sin(angle)
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="miter")

def draw_arrow(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    cx, cy = get_center(img_w, img_h)
    shaft_width = (min(img_w, img_h) - 2 * margin) / 5

    # Continuous arrow path (single polyline, no lift)
    points = [
        (cx - shaft_width, cy),
        (cx, margin),
        (cx + shaft_width, cy),
        (cx, cy),
        (cx, img_h - margin)
    ]
    draw.line(points, fill=LINE_COLOR, width=width, joint="miter")

def draw_circle(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    draw.ellipse([margin, margin, img_w - margin, img_h - margin], outline=LINE_COLOR, width=width)

def draw_oval(draw, width, img_w, img_h):
    margin_x = get_margin(img_w, img_h) * 2
    margin_y = get_margin(img_w, img_h) * 1.2
    draw.ellipse([margin_x, margin_y, img_w - margin_x, img_h - margin_y], outline=LINE_COLOR, width=width)

def draw_semicircle(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    draw.arc([margin, margin, img_w - margin, img_h - margin], start=0, end=180, fill=LINE_COLOR, width=width)

def draw_wave(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h)
    points = []
    num_points = 100
    amplitude = (min(img_w, img_h) - 2 * margin) / 4
    cy = img_h // 2
    for i in range(num_points):
        x = margin + (img_w - 2 * margin) * i / (num_points - 1)
        y = cy + amplitude * math.sin(2 * math.pi * i / (num_points / 3))
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="curve")

def draw_heart(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h) * 1.5
    cx, cy = get_center(img_w, img_h)
    size = min(img_w, img_h) - 2 * margin
    points = []
    num_points = 200
    for i in range(num_points + 1):
        t = 2 * math.pi * i / num_points
        x = 16 * math.sin(t) ** 3
        y = -(13 * math.cos(t) - 5 * math.cos(2*t) - 2 * math.cos(3*t) - math.cos(4*t))
        scale = size / 35
        x = cx + x * scale
        y = cy + y * scale - size / 8
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="curve")

def draw_spiral(draw, width, img_w, img_h):
    cx, cy = get_center(img_w, img_h)
    points = []
    num_points = 300
    max_radius = min(img_w, img_h) / 2 - get_margin(img_w, img_h)
    for i in range(num_points):
        angle = 4 * math.pi * i / num_points
        radius = max_radius * i / num_points
        x = cx + radius * math.cos(angle)
        y = cy + radius * math.sin(angle)
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="curve")

def draw_figure_eight(draw, width, img_w, img_h):
    cx, cy = get_center(img_w, img_h)
    points = []
    num_points = 200
    size = (min(img_w, img_h) - 2 * get_margin(img_w, img_h)) / 2
    for i in range(num_points + 1):
        t = 2 * math.pi * i / num_points
        x = size * math.cos(t)
        y = size * math.sin(t) * math.cos(t)
        points.append((cx + x, cy + y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="curve")

def draw_s_curve(draw, width, img_w, img_h):
    margin = get_margin(img_w, img_h)
    points = []
    num_points = 100
    for i in range(num_points):
        t = i / (num_points - 1)
        x = margin + (img_w - 2 * margin) * t
        y = margin + (img_h - 2 * margin) * (0.5 + 0.5 * math.sin(math.pi * (t - 0.5)))
        points.append((x, y))
    draw.line(points, fill=LINE_COLOR, width=width, joint="curve")

# ===== PATTERN DEFINITIONS =====

PATTERNS = {
    'level1_horizontal': ('Horizontal Line', draw_horizontal_line, 1),
    'level1_vertical': ('Vertical Line', draw_vertical_line, 1),
    'level1_diagonal': ('Diagonal Line', draw_diagonal_line, 1),
    'level1_zigzag': ('Zigzag', draw_zigzag, 1),
    'level2_square': ('Square', draw_square, 2),
    'level2_triangle': ('Triangle', draw_triangle, 2),
    'level2_rectangle': ('Rectangle', draw_rectangle, 2),
    'level2_cross': ('Cross', draw_cross, 2),
    'level3_pentagon': ('Pentagon', draw_pentagon, 3),
    'level3_hexagon': ('Hexagon', draw_hexagon, 3),
    'level3_star': ('Star', draw_star, 3),
    'level3_arrow': ('Arrow', draw_arrow, 3),
    'level4_circle': ('Circle', draw_circle, 4),
    'level4_oval': ('Oval', draw_oval, 4),
    'level4_semicircle': ('Semicircle', draw_semicircle, 4),
    'level4_wave': ('Wave', draw_wave, 4),
    'level5_heart': ('Heart', draw_heart, 5),
    'level5_spiral': ('Spiral', draw_spiral, 5),
    'level5_figure8': ('Figure Eight', draw_figure_eight, 5),
    'level5_scurve': ('S-Curve', draw_s_curve, 5),
}

def generate_pattern(pattern_key, draw_function, line_width_level, img_w, img_h):
    img = create_image(img_w, img_h)
    draw = ImageDraw.Draw(img)
    width = LINE_WIDTHS[line_width_level]
    draw_function(draw, width, img_w, img_h)
    return img

def generate_all_patterns():
    legacy_output_dir = "Assets/Resources/Patterns"
    if os.path.abspath(legacy_output_dir) != os.path.abspath(OUTPUT_DIR) and os.path.isdir(legacy_output_dir):
        print(f"[WARN] Legacy pattern folder also exists: {os.path.abspath(legacy_output_dir)}")
        print(f"       Unity project uses: {os.path.abspath(OUTPUT_DIR)}")
        print("       Make sure you are previewing textures from the Unity project folder.")
        print()

    os.makedirs(OUTPUT_DIR, exist_ok=True)
    total_patterns = 0
    print(f"Generating patterns in: {os.path.abspath(OUTPUT_DIR)}")
    print(f"Scale sizes: {[f'{w}x{h}' for w, h in SCALE_SIZES]}")
    print(f"Line width levels: {list(LINE_WIDTHS.values())}")
    print()
    for pattern_key, (name, draw_function, complexity) in PATTERNS.items():
        print(f"Generating {name} (Complexity Level {complexity})...")
        for width_level in range(1, 6):
            for scale_idx, (img_w, img_h) in enumerate(SCALE_SIZES):
                img = generate_pattern(pattern_key, draw_function, width_level, img_w, img_h)
                filename = f"scale_{img_w}x{img_h}_width_{width_level}_{pattern_key}.png"
                filepath = os.path.join(OUTPUT_DIR, filename)
                img.save(filepath)
                total_patterns += 1
                print(f"  [OK] Saved: {filename} (Width: {LINE_WIDTHS[width_level]}px, Scale: {img_w}x{img_h})")
    print()
    print(f"[SUCCESS] Complete! Generated {total_patterns} pattern images")
    print(f"Location: {os.path.abspath(OUTPUT_DIR)}")
    print()
    print("Next steps in Unity:")
    print("1. Refresh Unity (Ctrl+R or click Unity window)")
    print("2. Select all images in Assets/Resources/Patterns/")
    print("3. In Inspector, set 'Texture Type' to 'Sprite (2D and UI)'")
    print("4. Click 'Apply'")
    print()
    print("Pattern naming convention:")
    print("  scale_* = Image scale (resolution)")
    print("  width_1_* = Thinnest line (hardest)")
    print("  width_5_* = Thickest line (easiest)")
    print("  level1_* = Simple straight lines")
    print("  level2_* = Simple shapes")
    print("  level3_* = Complex shapes")
    print("  level4_* = Curved shapes")
    print("  level5_* = Complex curves")

if __name__ == "__main__":
    print("=" * 60)
    print("Quest VR Tracing App - Pattern Generator")
    print("=" * 60)
    print()
    
    try:
        generate_all_patterns()
    except Exception as e:
        print(f"[ERROR] Error: {e}")
        import traceback
        traceback.print_exc()
