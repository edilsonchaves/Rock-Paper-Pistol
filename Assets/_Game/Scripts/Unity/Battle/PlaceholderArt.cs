using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity.Battle
{
    public static class PlaceholderArt
    {
        public static Sprite Table()
        {
            return Make(512, 220, tex =>
            {
                Fill(tex, new Color(0.18f, 0.18f, 0.19f));
                FillRect(tex, 20, 20, 472, 180, new Color(0.42f, 0.42f, 0.43f));
            });
        }

        public static Sprite Slot()
        {
            return Make(140, 190, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                DrawRectOutline(tex, 8, 8, 124, 174, new Color(0.92f, 0.92f, 0.92f, 0.7f), 4);
            });
        }

        public static Sprite CardBack()
        {
            return Make(160, 220, tex =>
            {
                Fill(tex, new Color(0.22f, 0.13f, 0.08f));
                DrawRectOutline(tex, 6, 6, 148, 208, new Color(0.45f, 0.28f, 0.14f), 6);
                FillCircle(tex, 80, 110, 34, new Color(0.38f, 0.22f, 0.12f));
            });
        }

        public static Sprite CardFront(Card card, Suit displaySuit)
        {
            return Make(160, 220, tex =>
            {
                Color paper = card.IsPistol
                    ? new Color(0.86f, 0.74f, 0.52f)
                    : SuitPaper(displaySuit);
                Fill(tex, paper);
                DrawRectOutline(tex, 4, 4, 152, 212, Color.black, 4);

                if (card.IsPistol)
                {
                    DrawDiamond(tex, 80, 108, 46, new Color(0.75f, 0.58f, 0.28f));
                    DrawGun(tex, 80, 108, Color.black);
                }
                else
                {
                    DrawSuitIcon(tex, displaySuit, 80, 100);
                }
            });
        }

        public static Sprite Opponent()
        {
            return Make(220, 260, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                FillCircle(tex, 110, 175, 48, new Color(0.85f, 0.85f, 0.85f));
                FillRect(tex, 55, 20, 110, 140, new Color(0.12f, 0.12f, 0.12f));
                FillCircle(tex, 128, 180, 10, Color.white);
                FillCircle(tex, 128, 180, 4, Color.black);
            });
        }

        public static Sprite CircleEmpty()
        {
            return Make(36, 36, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                DrawCircleOutline(tex, 18, 18, 13, new Color(0.65f, 0.65f, 0.65f), 3);
            });
        }

        public static Sprite FacePlayer()
        {
            return Make(36, 36, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                FillCircle(tex, 18, 18, 14, new Color(0.82f, 0.82f, 0.84f));
                FillCircle(tex, 13, 20, 2, Color.black);
                FillCircle(tex, 23, 20, 2, Color.black);
            });
        }

        public static Sprite FaceEnemy()
        {
            return Make(36, 36, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                FillCircle(tex, 18, 18, 14, new Color(0.18f, 0.18f, 0.18f));
                FillCircle(tex, 22, 20, 3, Color.white);
            });
        }

        public static Sprite CircleDraw()
        {
            return Make(36, 36, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                DrawCircleOutline(tex, 18, 18, 13, new Color(0.45f, 0.45f, 0.45f), 3);
                DrawLine(tex, 10, 10, 26, 26, new Color(0.55f, 0.55f, 0.55f), 2);
            });
        }

        public static Sprite GoldOutline()
        {
            return Make(176, 236, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                DrawRectOutline(tex, 4, 4, 168, 228, new Color(0.93f, 0.75f, 0.18f), 8);
            });
        }

        public static Sprite Bubble()
        {
            return Make(220, 70, tex =>
            {
                Fill(tex, new Color(1f, 1f, 1f, 0f));
                FillRect(tex, 8, 12, 204, 50, Color.white);
            });
        }

        public static AudioClip Tone(string name, float frequency, float seconds)
        {
            const int sampleRate = 22050;
            int samples = Mathf.Max(64, (int)(sampleRate * seconds));
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float envelope = 1f - i / (float)samples;
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.32f;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static Color SuitPaper(Suit suit)
        {
            switch (suit)
            {
                case Suit.Rock:
                    return new Color(0.62f, 0.62f, 0.64f);
                case Suit.Scissors:
                    return new Color(0.90f, 0.74f, 0.55f);
                default:
                    return new Color(0.95f, 0.95f, 0.93f);
            }
        }

        private static void DrawSuitIcon(Texture2D tex, Suit suit, int cx, int cy)
        {
            switch (suit)
            {
                case Suit.Rock:
                    FillCircle(tex, cx, cy, 28, new Color(0.28f, 0.28f, 0.30f));
                    break;
                case Suit.Paper:
                    FillRect(tex, cx - 22, cy - 28, 44, 56, Color.white);
                    DrawRectOutline(tex, cx - 22, cy - 28, 44, 56, Color.black, 2);
                    break;
                default:
                    DrawLine(tex, cx - 20, cy + 18, cx, cy - 22, Color.black, 4);
                    DrawLine(tex, cx + 20, cy + 18, cx, cy - 22, Color.black, 4);
                    DrawLine(tex, cx - 22, cy + 20, cx + 22, cy + 20, Color.black, 4);
                    break;
            }
        }

        private static void DrawGun(Texture2D tex, int cx, int cy, Color color)
        {
            FillRect(tex, cx - 28, cy, 50, 10, color);
            FillRect(tex, cx + 10, cy - 18, 10, 20, color);
        }

        private static Sprite Make(int width, int height, System.Action<Texture2D> paint)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            Fill(tex, Color.clear);
            paint(tex);
            tex.Apply();
            return Sprite.Create(
                tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect);
        }

        public static void ApplyVisibleMaterial(SpriteRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            Material material = VisibleSpriteMaterial();
            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Material _visibleSpriteMaterial;

        private static Material VisibleSpriteMaterial()
        {
            if (_visibleSpriteMaterial != null)
            {
                return _visibleSpriteMaterial;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            if (shader == null)
            {
                return null;
            }

            _visibleSpriteMaterial = new Material(shader);
            return _visibleSpriteMaterial;
        }

        private static void Fill(Texture2D tex, Color color)
        {
            Color[] pixels = new Color[tex.width * tex.height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            tex.SetPixels(pixels);
        }

        private static void FillRect(Texture2D tex, int x, int y, int w, int h, Color color)
        {
            for (int py = y; py < y + h; py++)
            {
                for (int px = x; px < x + w; px++)
                {
                    Plot(tex, px, py, color);
                }
            }
        }

        private static void DrawRectOutline(Texture2D tex, int x, int y, int w, int h, Color color, int thickness)
        {
            FillRect(tex, x, y, w, thickness, color);
            FillRect(tex, x, y + h - thickness, w, thickness, color);
            FillRect(tex, x, y, thickness, h, color);
            FillRect(tex, x + w - thickness, y, thickness, h, color);
        }

        private static void FillCircle(Texture2D tex, int cx, int cy, int radius, Color color)
        {
            int r2 = radius * radius;
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= r2)
                    {
                        Plot(tex, cx + x, cy + y, color);
                    }
                }
            }
        }

        private static void DrawCircleOutline(Texture2D tex, int cx, int cy, int radius, Color color, int thickness)
        {
            for (int t = 0; t < thickness; t++)
            {
                int r = radius - t;
                int r2 = r * r;
                int inner = (r - 1) * (r - 1);
                for (int y = -r; y <= r; y++)
                {
                    for (int x = -r; x <= r; x++)
                    {
                        int d = x * x + y * y;
                        if (d <= r2 && d >= inner)
                        {
                            Plot(tex, cx + x, cy + y, color);
                        }
                    }
                }
            }
        }

        private static void DrawDiamond(Texture2D tex, int cx, int cy, int size, Color color)
        {
            for (int y = -size; y <= size; y++)
            {
                int span = size - Mathf.Abs(y);
                for (int x = -span; x <= span; x++)
                {
                    Plot(tex, cx + x, cy + y, color);
                }
            }
        }

        private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color color, int thickness)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                FillCircle(tex, x0, y0, thickness, color);
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private static void Plot(Texture2D tex, int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= tex.width || y >= tex.height)
            {
                return;
            }

            tex.SetPixel(x, y, color);
        }
    }
}
