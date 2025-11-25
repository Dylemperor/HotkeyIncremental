# Prestige System Setup Guide

This guide will help you set up the prestige/reset system in Unity.

## Overview

The prestige system has been implemented with:
- **Silver Prestige**: Appears when player reaches 100 Billion Z currency. Gives 2x production boost per letter plated.
- **Gold Prestige**: Appears after all letters are silver plated. Gives 5x production boost per letter plated.

## Unity Editor Setup

### Step 1: Add PrestigeManager Component

1. In your scene, find or create a GameObject (can be the same as GameController or a new one)
2. Add the `PrestigeManager` component to it
3. In the Inspector, assign:
   - **Currency Manager**: Drag your CurrencyManager component
   - **Unlock Manager**: Drag your UnlockManager component
   - **Silver Prestige Requirement**: 100000000000 (100 Billion - already set)
   - **Silver Multiplier**: 2.0 (already set)
   - **Gold Multiplier**: 5.0 (already set)

### Step 2: Create Prestige Buttons in Keyboard View

1. Open your **Keyboard View** GameObject in the scene
2. Create two new UI Buttons:
   - Right-click on Keyboard View → UI → Button
   - Name the first one "Silver Prestige Button"
   - Name the second one "Gold Prestige Button"

3. **Position the buttons**:
   - Place the Silver button where you want it in the keyboard view
   - Place the Gold button directly below the Silver button (as specified)

4. **Configure Silver Prestige Button**:
   - Select the Silver Prestige Button
   - In the Inspector, find the Button component
   - Add a TextMeshPro - Text component as a child (if not already present) for the button text
   - The button will be automatically styled with a silver color

5. **Configure Gold Prestige Button**:
   - Select the Gold Prestige Button
   - In the Inspector, find the Button component
   - Add a TextMeshPro - Text component as a child (if not already present) for the button text
   - The button will be automatically styled with a gold color

### Step 3: Add PrestigeUI Component

1. Create a new GameObject or use an existing UI manager GameObject
2. Add the `PrestigeUI` component to it
3. In the Inspector, assign:
   - **Prestige Manager**: Drag your PrestigeManager component
   - **Currency Manager**: Drag your CurrencyManager component
   - **Silver Prestige Button**: Drag the Silver Prestige Button you created
   - **Silver Button Text**: Drag the TextMeshPro component from the Silver button
   - **Silver Button Image**: Drag the Image component from the Silver button (usually the button itself)
   - **Gold Prestige Button**: Drag the Gold Prestige Button you created
   - **Gold Button Text**: Drag the TextMeshPro component from the Gold button
   - **Gold Button Image**: Drag the Image component from the Gold button (usually the button itself)
   - **Update Interval**: 0.1 (default - updates 10 times per second)

4. **Connect Button Click Events**:
   - Select the Silver Prestige Button
   - In the Button component's OnClick() event, click the "+" button
   - Drag the GameObject with PrestigeUI component to the object field
   - Select `PrestigeUI → OnSilverPrestigeClick()` from the dropdown
   
   - Select the Gold Prestige Button
   - In the Button component's OnClick() event, click the "+" button
   - Drag the GameObject with PrestigeUI component to the object field
   - Select `PrestigeUI → OnGoldPrestigeClick()` from the dropdown

### Step 4: Making Buttons Shiny (Optional Enhancement)

To make the buttons look more shiny/metallic:

1. **For Silver Button**:
   - Select the Silver Prestige Button
   - Add a **Shadow** component (Add Component → UI → Shadow)
   - Set Shadow Effect Color to a light blue-white: `(0.9, 0.9, 1.0, 0.5)`
   - Set Effect Distance to `(2, -2)`
   - Optionally add an **Outline** component for extra shine
   - You can also create a custom material with a metallic shader if desired

2. **For Gold Button**:
   - Select the Gold Prestige Button
   - Add a **Shadow** component
   - Set Shadow Effect Color to a warm yellow: `(1.0, 0.85, 0.3, 0.5)`
   - Set Effect Distance to `(2, -2)`
   - Optionally add an **Outline** component
   - Consider using a gradient or custom shader for a more gold-like appearance

### Step 5: Update GameController (Optional)

If you want to reference PrestigeManager in GameController:

1. Open GameController script
2. Add a public field: `public PrestigeManager prestigeManager;`
3. In the Unity Inspector, assign the PrestigeManager component

## How It Works

1. **Silver Prestige**:
   - Button appears when Z currency reaches 100 Billion
   - Clicking resets all currencies to 0
   - One letter (in unlock order) gains silver plating and 2x production boost
   - Each reset plates one more letter
   - Letters turn silver-colored in the keyboard view

2. **Gold Prestige**:
   - Button appears when ALL letters are silver plated
   - Clicking resets all currencies to 0
   - One letter (in unlock order) gains gold plating and 5x production boost
   - Gold replaces silver (if a letter was silver, it becomes gold)
   - Letters turn gold-colored in the keyboard view

3. **Production Multipliers**:
   - Silver: 2x per letter
   - Gold: 5x per letter (replaces silver multiplier)
   - Multipliers stack (if you have 3 silver letters, production is 2x * 2x * 2x = 8x for those letters)

## Testing

1. Use the Unity Editor's Play mode
2. You can temporarily lower the `silverPrestigeRequirement` in PrestigeManager to test (e.g., set to 1000)
3. Check that buttons appear/disappear correctly
4. Verify that letters change color when plated
5. Verify that production rates increase after prestige

## Save/Load

The prestige system automatically saves and loads:
- Silver/Gold status for each letter
- Prestige multipliers
- Plated counts

Data is saved to PlayerPrefs and persists between game sessions.

## Notes

- Prestige buttons are hidden by default and only appear when requirements are met
- The system tracks plating progress (e.g., "3/26 Plated")
- Letters are plated in unlock order (A, B, C, ..., Z)
- Production multipliers are applied automatically in ProductionManager

