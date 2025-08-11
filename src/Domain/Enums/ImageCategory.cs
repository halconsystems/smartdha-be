using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum ImageCategory
{
    Main = 0,          // Primary display image (thumbnail/card)
    Front = 1,         // Front/entrance view
    Back = 2,          // Back side
    Left = 3,          // Left side
    Right = 4,         // Right side
    Interior = 5,      // Room interior (general)
    Bed = 6,           // Bed/ sleeping area
    Bathroom = 7,      // Bathroom/Washroom
    Kitchenette = 8,   // Kitchen/Kitchenette
    Balcony = 9,       // Balcony/Terrace
    View = 10,         // Window/Outside view
    Amenities = 11,    // In-room amenities (kettle, TV, etc.)
    FloorPlan = 12,    // Plan/diagram
    Panorama360 = 13,  // 360° images
    Other = 99         // Anything else
}

