# Card Sprites Setup Guide

## Sprite Requirements

### Card Dimensions
- **Recommended Size**: 140x190 pixels (standard playing card ratio)
- **Alternative Sizes**:
  - Small: 70x95 px
  - Medium: 140x190 px
  - Large: 280x380 px
- **Ratio**: Always maintain 1:1.36 (width:height)

### File Format
- **Format**: PNG with transparency
- **Color Mode**: RGBA
- **DPI**: 72 for screen, 300 for print quality

## Naming Convention

### Card Faces
Use this naming pattern: `{suit}_{rank}.png`

**Suits**: clubs, diamonds, hearts, spades
**Ranks**: ace, 2, 3, 4, 5, 6, 7, 8, 9, 10, jack, queen, king

**Examples:**
```
clubs_ace.png
clubs_2.png
hearts_king.png
spades_10.png
diamonds_queen.png
```

### Card Backs
```
card_back_blue.png
card_back_red.png
card_back_default.png
```

## Folder Structure

```
Assets/
  Art/
    Cards/
      Faces/
        Clubs/
          clubs_ace.png
          clubs_2.png
          ...
        Diamonds/
          diamonds_ace.png
          ...
        Hearts/
          hearts_ace.png
          ...
        Spades/
          spades_ace.png
          ...
      Backs/
        card_back_blue.png
        card_back_red.png
      Effects/
        card_shadow.png
        card_glow.png
```

## Unity Import Settings

### For Card Sprites
1. Select all card sprites in Project window
2. In Inspector:
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Single
   - **Pixels Per Unit**: 100
   - **Filter Mode**: Bilinear
   - **Compression**: None (best quality) or High Quality
   - **Max Size**: 512 or 1024

### For Sprite Atlas (Performance)
Create a Sprite Atlas for better performance:

1. Right-click in Project → Create → 2D → Sprite Atlas
2. Name it "CardsAtlas"
3. Add settings:
   - **Include in Build**: Checked
   - **Allow Rotation**: Unchecked
   - **Tight Packing**: Checked
   - **Padding**: 2
4. Add Objects for Packing → Add your Cards folder

## Free Sprite Resources

### 1. Kenney's Boardgame Pack
- URL: https://kenney.nl/assets/boardgame-pack
- License: CC0 (Public Domain)
- Includes: Complete card deck + extras

### 2. OpenGameArt
- URL: https://opengameart.org
- Search: "playing cards" or "card deck"
- Various licenses (check each)

### 3. itch.io Free Assets
- URL: https://itch.io/game-assets/free/tag-cards
- Filter by Free assets
- Various styles available

### 4. Vector Card Decks
- Search "SVG playing cards" on GitHub
- Convert SVG to PNG using:
  - Inkscape (free)
  - Online converters
  - ImageMagick (command line)

## Creating Simple Test Sprites

If you want to quickly generate test sprites with code:

### Python Script (Pillow library)
```python
from PIL import Image, ImageDraw, ImageFont
import os

# Card dimensions
WIDTH, HEIGHT = 140, 190
SUITS = ['clubs', 'diamonds', 'hearts', 'spades']
RANKS = ['ace', '2', '3', '4', '5', '6', '7', '8', '9', '10', 'jack', 'queen', 'king']
SUIT_SYMBOLS = {
    'clubs': '♣',
    'diamonds': '♦',
    'hearts': '♥',
    'spades': '♠'
}

def create_card(suit, rank, output_path):
    # Create white card with border
    img = Image.new('RGBA', (WIDTH, HEIGHT), 'white')
    draw = ImageDraw.Draw(img)

    # Draw border
    draw.rectangle([0, 0, WIDTH-1, HEIGHT-1], outline='black', width=2)

    # Draw suit symbol and rank
    font = ImageFont.truetype('/System/Library/Fonts/Helvetica.ttc', 24)
    symbol = SUIT_SYMBOLS[suit]
    text = f"{rank.upper()}\n{symbol}"

    # Color for suit
    color = 'red' if suit in ['hearts', 'diamonds'] else 'black'

    # Draw text
    draw.text((10, 10), text, fill=color, font=font)

    # Save
    img.save(output_path)

# Generate all cards
os.makedirs('cards/faces', exist_ok=True)
for suit in SUITS:
    for rank in RANKS:
        filename = f'cards/faces/{suit}_{rank}.png'
        create_card(suit, rank, filename)
        print(f'Created {filename}')

# Create card back
back = Image.new('RGBA', (WIDTH, HEIGHT), 'blue')
draw = ImageDraw.Draw(back)
draw.rectangle([0, 0, WIDTH-1, HEIGHT-1], outline='white', width=3)
back.save('cards/backs/card_back_blue.png')
print('Created card back')
```

### Using Online Tools
- **Figma** (free): Design cards visually
- **Canva** (free): Use templates
- **Photopea** (free online Photoshop): Draw custom cards

## Integration with Unity

### Method 1: Direct Assignment (Simple)
In Unity Editor, manually drag sprites to Card prefab:
1. Select Card prefab
2. Expand CardVisual child
3. Drag sprite to SpriteRenderer component

### Method 2: Resources Folder (Dynamic Loading)
```
Assets/
  Resources/
    Cards/
      Faces/
        clubs_ace.png
        ...
```

Then load in C#:
```csharp
Sprite sprite = Resources.Load<Sprite>($"Cards/Faces/{suit}_{rank}");
```

### Method 3: Addressables (Advanced, Best for Production)
Use Unity's Addressable Assets system for better memory management:
1. Install Addressables package
2. Mark sprites as Addressable
3. Load async in code

## Next Steps

1. Choose or create your sprite set
2. Import into Unity
3. Update Card.cs to use sprites
4. Test in game
5. Add polish (animations, effects)

## Sprite Quality Guidelines

### For WebGL Build
- Keep sprites reasonably sized (140x190 or 280x380 max)
- Use Sprite Atlas to reduce draw calls
- Consider texture compression for final build

### For Mobile Build
- Use lower resolution sprites (70x95 or 140x190)
- Enable texture compression
- Limit atlas size to 2048x2048

### For Desktop Build
- Can use higher quality sprites (280x380)
- Less concern about file size
- Focus on visual quality
