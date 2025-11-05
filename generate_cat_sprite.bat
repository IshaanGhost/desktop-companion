@echo off
REM Script to generate cat sprite sheet
REM Requires Python and PIL/Pillow

echo Installing/updating Pillow if needed...
pip install Pillow --quiet

echo.
echo Generating cat sprite sheet...
python generate_cat_sprite.py

pause

