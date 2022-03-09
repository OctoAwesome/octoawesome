### Cooking Recipe Sample
``` json
{
    "Name": "OctoAwesome.Basics:CookMeat",
    "Type": "Furnace", //, Crafting, etc.pp. Freitext
    "Inputs":[
        {
            "ItemName":"RawMeat",
            "Count":1
        }
    ],
    "Outputs":[
        {
            "ItemName":"CookedMeat",
            "Count":1
        }
    ],
    "Time":"900",
    "MinTime":"180",
    "Category":"Cooking", //Isn't that type, but different?
    "SubCategory": "Food",
    "Enabled":true, //Selfexplenatory
    "Description":"Cooks raw meat into cooked meat", //INTL Text for user
    "Keywords":"Cooking,CookedMeat,RawMeat" //Search, Filter
}
```

### Iron Smelting Recipe Sample
``` json
{
    "Type": "Blast Furnace", //, Crafting, etc.pp. Freitext
    "Inputs":[
        {
            "ItemName":"IronOre",
            "Count":5
        }
    ],
    "Outputs":[
        {
            "ItemName":"Iron",
            "Count":2,
        },
        {
            "ItemName":"Slag",
            "Count":3,
        }
    ],
    "Energy":"70",
    "Category":"Smelting", //Isn't that type, but different?
    "SubCategory": "Metal",
    "Enabled":true, //Selfexplenatory
    "Description":"Smelts Iron Ore into Iron", //INTL Text for user
    "Keywords":"Smelting,IronOre,Iron" //Search, Filter
}
```

1.4 W / g, 0.070min Schmelzzeit

31MJ/kg Koks
25t Eisen

8060kg Koks / 1t Eisen

____

* Energy: 500 wh für 200g Fleisch
* Zeit:   15 Minuten für 200g Fleisch
* Combined: Mind. 3 Minuten, Zeit 15 Minuten XOR 500 wh für 200g Fleisch

____
Entscheidung:
- Required: Zeit XOR Energy
- Optional: Mindestzeit

Rausgefallen:
- Required: Mindestzeit
- Optional: Energy OR Zeit 