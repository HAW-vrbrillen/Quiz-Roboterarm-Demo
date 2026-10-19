import json, math, sys
from PIL import Image, ImageDraw, ImageFont

D = sys.argv[1]; OUT = sys.argv[2]
W = H = 2048
F = "/System/Library/Fonts/Supplemental/"
def font(sz, bold=False): return ImageFont.truetype(F + ("Arial Bold.ttf" if bold else "Arial.ttf"), sz)

countries = json.load(open(D + "/countries.geojson"))["features"]
admin1 = json.load(open(D + "/admin1.geojson"))["features"]
lakes = json.load(open(D + "/lakes.geojson"))["features"]

# Kartenausschnitt (Grad) und Fläche im Bild
LON0, LON1, LAT0, LAT1 = 10.5, 33.0, -14.5, 6.5
MX0, MY0, MX1, MY1 = 70, 230, 1978, 1840
sx = (MX1 - MX0) / (LON1 - LON0)
sy = (MY1 - MY0) / (LAT1 - LAT0)
s = min(sx, sy)
ox = MX0 + ((MX1 - MX0) - s * (LON1 - LON0)) / 2
oy = MY0 + ((MY1 - MY0) - s * (LAT1 - LAT0)) / 2
def P(lon, lat): return (ox + (lon - LON0) * s, oy + (LAT1 - lat) * s)

def polys(geom):
    if geom["type"] == "Polygon": return [geom["coordinates"]]
    if geom["type"] == "MultiPolygon": return geom["coordinates"]
    return []

img = Image.new("RGB", (W, H), (255, 255, 255))
d = ImageDraw.Draw(img)
WASSER = (205, 226, 240)
d.rectangle([ox, oy, ox + s * (LON1 - LON0), oy + s * (LAT1 - LAT0)], fill=WASSER)  # Ozean

def zeichne(feat, fill, outline=(120, 120, 120), width=2):
    for poly in polys(feat["geometry"]):
        ring = [P(x, y) for x, y in poly[0]]
        if len(ring) > 2: d.polygon(ring, fill=fill, outline=outline, width=width)
        for loch in poly[1:]:
            r = [P(x, y) for x, y in loch]
            if len(r) > 2: d.polygon(r, fill=WASSER)

for f in countries:
    if f["properties"]["ADM0_A3"] != "COD":
        zeichne(f, (232, 232, 228), (150, 150, 150), 2)
cod = [f for f in countries if f["properties"]["ADM0_A3"] == "COD"][0]
zeichne(cod, (250, 247, 232), (60, 60, 60), 2)

KONFLIKT = (232, 120, 110); KOBALT = (120, 160, 220)
for f in admin1:
    p = f["properties"]
    if p.get("adm0_a3") != "COD": continue
    farbe = None
    if p["name"] in ("Nord-Kivu", "Sud-Kivu"): farbe = KONFLIKT
    if p["name"] == "Katanga": farbe = KOBALT
    zeichne(f, farbe or (250, 247, 232), (175, 170, 150), 1)
zeichne(cod, None, (40, 40, 40), 5)  # Landesgrenze kräftig obendrauf

for f in lakes:
    if f["properties"].get("scalerank", 9) <= 4:
        zeichne(f, WASSER, (110, 150, 185), 1)

# Nachbarländer beschriften
nachbarn = {"Ruanda": (30.25, -2.55), "Uganda": (32.3, 1.6), "Burundi": (30.35, -3.55), "Tansania": (32.2, -6.8),
            "Sambia": (27.5, -13.5), "Angola": (17.3, -11.5), "Republik\nKongo": (14.6, -0.7),
            "Zentralafrikanische\nRepublik": (20.6, 5.6), "Südsudan": (30.5, 5.6)}
for n, (lo, la) in nachbarn.items():
    x, y = P(lo, la)
    d.multiline_text((x, y), n, fill=(105, 105, 105), font=font(30), anchor="mm", align="center")

# Städte
staedte = [("Kinshasa", 15.31, -4.32, "Hauptstadt", "r"), ("Goma", 29.22, -1.68, "", "r"), ("Bukavu", 28.86, -2.51, "", "l"),
           ("Rubaya (Coltan-Mine)", 28.88, -1.45, "", "l"), ("Lubumbashi", 27.48, -11.66, "", "r"), ("Kolwezi", 25.47, -10.72, "", "l")]
for name, lo, la, zusatz, seite in staedte:
    x, y = P(lo, la)
    r = 13 if name == "Kinshasa" else 10
    if name == "Kinshasa":
        d.rectangle([x - r, y - r, x + r, y + r], fill=(20, 20, 20))
    else:
        d.ellipse([x - r, y - r, x + r, y + r], fill=(20, 20, 20), outline=(255, 255, 255), width=3)
    f = font(36, True)
    if seite == "r": d.text((x + 20, y), name, fill=(20, 20, 20), font=f, anchor="lm")
    else: d.text((x - 20, y), name, fill=(20, 20, 20), font=f, anchor="rm")

# Regionsbeschriftungen
x, y = P(24.6, -0.3)
d.multiline_text((x, y), "Konfliktgebiet\nNord- und Süd-Kivu", fill=(150, 30, 25), font=font(38, True), anchor="mm", align="center")
x, y = P(26.4, -9.1)
d.multiline_text((x, y), "Kobalt- und Kupferabbau\n(früher Provinz Katanga)", fill=(25, 60, 130), font=font(38, True), anchor="mm", align="center")
x, y = P(21.0, -2.6)
d.text((x, y), "DR KONGO", fill=(70, 70, 70), font=font(64, True), anchor="mm")

# Karte zuschneiden: alles außerhalb des Kartenrahmens weiß
KX0, KY0, KX1, KY1 = 40, 230, W - 40, 1925
d.rectangle([0, 0, W, KY0], fill=(255, 255, 255)); d.rectangle([0, KY1, W, H], fill=(255, 255, 255))
d.rectangle([0, 0, KX0, H], fill=(255, 255, 255)); d.rectangle([KX1, 0, W, H], fill=(255, 255, 255))
d.rectangle([KX0, KY0, KX1, KY1], outline=(150, 150, 150), width=2)
# Titel
d.text((70, 60), "Demokratische Republik Kongo", fill=(43, 138, 43), font=font(78, True))
d.text((72, 160), "Fläche: 2,3 Mio. km² – mehr als sechsmal so groß wie Deutschland", fill=(30, 30, 30), font=font(40))

# Deutschland im gleichen Maßstab (unten links, ins Meer)
de = [f for f in countries if f["properties"]["ADM0_A3"] == "DEU"][0]
cx, cy = 10.4, 51.2
k = math.cos(math.radians(cy)) / math.cos(math.radians(-4))  # Längengrade bei 51° N sind kürzer
kb = [KX0 + 30, int(KY1 - 30 - (8.0 * s + 110)), KX0 + 520, KY1 - 30]
d.rounded_rectangle(kb, radius=14, fill=(255, 255, 255), outline=(170, 170, 170), width=2)
bx, by = (kb[0] + kb[2]) / 2, kb[1] + 25 + 3.95 * s
for poly in polys(de["geometry"]):
    ring = [(bx + (lo - cx) * s * k, by + (cy - la) * s) for lo, la in poly[0]]
    d.polygon(ring, fill=(255, 210, 90), outline=(120, 90, 20), width=3)
d.multiline_text((bx, kb[3] - 20), "Deutschland\nim gleichen Maßstab", fill=(90, 70, 20), font=font(30, True), anchor="md", align="center")

# Legende
lx, ly = KX0 + 50, KY0 + 50
d.rounded_rectangle([lx - 20, ly - 20, lx + 500, ly + 5 * 62 + 4], radius=14, fill=(255, 255, 255), outline=(170, 170, 170), width=2)
def eintrag(i, farbe, text, form="box"):
    y = ly + i * 62
    if form == "box": d.rectangle([lx, y, lx + 46, y + 40], fill=farbe, outline=(90, 90, 90))
    elif form == "quad": d.rectangle([lx + 10, y + 7, lx + 36, y + 33], fill=farbe)
    else: d.ellipse([lx + 10, y + 7, lx + 36, y + 33], fill=farbe)
    d.text((lx + 66, y + 20), text, fill=(30, 30, 30), font=font(34), anchor="lm")
eintrag(0, KONFLIKT, "Konfliktgebiet im Osten")
eintrag(1, KOBALT, "Kobalt- und Kupferabbau")
eintrag(2, (20, 20, 20), "Hauptstadt", "quad")
eintrag(3, (20, 20, 20), "Stadt / Mine", "punkt")
eintrag(4, WASSER, "Seen")

d.text((70, 1985), "Kartendaten: Natural Earth (gemeinfrei). Provinzgrenzen vor der Neuordnung 2015; Lage der Orte vereinfacht.",
       fill=(110, 110, 110), font=font(27), anchor="ls")
img.save(OUT)
print("ok", OUT)
