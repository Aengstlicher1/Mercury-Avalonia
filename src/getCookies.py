import sqlite3
import shutil
import os

profile_path = os.path.expandvars(r"%APPDATA%\Mozilla\Firefox\Profiles\5e4zflpb.default-release-1757319172636")
cookies_db = os.path.join(profile_path, "cookies.sqlite")

tmp_db = os.path.join(os.environ["TEMP"], "ff_cookies_copy.sqlite")
shutil.copy2(cookies_db, tmp_db)

conn = sqlite3.connect(tmp_db)
cursor = conn.cursor()

# Only get cookies for .youtube.com (not subdomains like history.youtube.com etc.)
cursor.execute("""
    SELECT name, value FROM moz_cookies 
    WHERE host IN ('.youtube.com', 'music.youtube.com')
    ORDER BY name
""")

cookies = cursor.fetchall()
conn.close()
os.remove(tmp_db)

# Filter out junk
skip_prefixes = ("ST-", "CONSISTENCY")
seen = set()
filtered = []

for name, value in cookies:
    if name.startswith(skip_prefixes):
        continue
    if name in seen:
        continue
    seen.add(name)
    filtered.append((name, value))

cookie_string = "; ".join(f"{name}={value}" for name, value in filtered)

print("=== Clean Cookie String ===\n")
print(cookie_string)
print(f"\n\n=== {len(filtered)} cookies (filtered from {len(cookies)}) ===")
