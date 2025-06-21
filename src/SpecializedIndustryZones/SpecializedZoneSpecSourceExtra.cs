using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colossal.Logging;
using Game.Economy;
using Newtonsoft.Json;
using UnityEngine;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpecSourceExtra
{
    public static readonly string officeByEducation = """
    {
        "Version": "v1alpha1",
            "Zones": {
            "EducatedOfficeHigh": {
                "Name": "Educated High Density Offices",
                    "BaseZoneName": "Office High",
                        "Description": "High density offices that require mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Financial, Media.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_financial.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Financial",
                                "Media",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "WellEducatedOfficeHigh": {
                "Name": "Well Educated High Density Offices",
                    "BaseZoneName": "Office High",
                        "Description": "High density offices that require mostly Well Educated workers at Level 1 and mostly Highly Educated workers at Level 5 - Software, Telecom.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_software.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Software",
                                "Telecom",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EducatedOfficeLow": {
                "Name": "Educated Low Density Offices",
                    "BaseZoneName": "Office Low",
                        "Description": "Low density offices that require mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Financial, Media.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_financial.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Financial",
                                "Media",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "WellEducatedOfficeLow": {
                "Name": "Well Educated Low Density Offices",
                    "BaseZoneName": "Office Low",
                        "Description": "Low density offices that require mostly Well Educated workers at Level 1 and mostly Highly Educated workers at Level 5 - Software, Telecom.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_software.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Software",
                                "Telecom",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
        }
    }
    """;
    public static readonly string industryByEducation = """
    {
        "Version": "v1alpha1",
            "Zones": {
            "UneducatedManufacturing": {
                "Name": "Uneducated Manufacturing",
                    "BaseZoneName": "Industrial Manufacturing",
                        "Description": "Industry manufacturing that requires mostly Uneducated workers at Level 1 and mostly Poorly Educated workers at Level 5 - Timber.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_timber.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Timber",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "PoorlyEducatedManufacturing": {
                "Name": "Poorly Educated Manufacturing",
                    "BaseZoneName": "Industrial Manufacturing",
                        "Description": "Industry manufacturing that requires mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Furniture, Textiles, Food, Convenience Food, Beverages, Machinery, Concrete, Minerals, Metals.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Furniture",
                                "Textiles",
                                "Food",
                                "ConvenienceFood",
                                "Beverages",
                                "Machinery",
                                "Concrete",
                                "Minerals",
                                "Metals",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EducatedManufacturing": {
                "Name": "Educated Manufacturing",
                    "BaseZoneName": "Industrial Manufacturing",
                        "Description": "Industry manufacturing that requires mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Paper, Vehicles, Electronics, Plastics, Chemicals, Petrochemicals sourced from Crude Oil, Steel.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Paper",
                                "Vehicles",
                                "Electronics",
                                "Plastics",
                                "Chemicals",
                                "Petrochemicals",
                                "Steel",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "WellEducatedManufacturing": {
                "Name": "Well Educated Manufacturing",
                    "BaseZoneName": "Industrial Manufacturing",
                        "Description": "Industry manufacturing that requires mostly Well Educated workers at Level 1 and mostly Highly Educated workers at Level 5 - Pharmaceuticals, Petrochemicals sourced from Grain.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_pharmaceuticals.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [
                                "Pharmaceuticals",
                                "Petrochemicals",
                            ],
                            "StoredResources": [],
                            "SoldResources": [],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
        }
    }
    """;
    public static readonly string commercialByEducation = """
    {
        "Version": "v1alpha1",
            "Zones": {
            "EU_PoorlyEducatedLowDensityCommercial": {
                "Name": "EU Poorly Educated Low Density Business",
                    "BaseZoneName": "EU Commercial Low",
                        "Description": "Low density businesses that require mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_EducatedLowDensityCommercial": {
                "Name": "EU Educated Low Density Business",
                    "BaseZoneName": "EU Commercial Low",
                        "Description": "Low density businesses that require mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_PoorlyEducatedLowDensityCommercial": {
                "Name": "NA Poorly Educated Low Density Business",
                    "BaseZoneName": "NA Commercial Low",
                        "Description": "Low density businesses that require mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_EducatedLowDensityCommercial": {
                "Name": "NA Educated Low Density Business",
                    "BaseZoneName": "NA Commercial Low",
                        "Description": "Low density businesses that require mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_PoorlyEducatedHighDensityCommercial": {
                "Name": "EU Poorly Educated High Density Business",
                    "BaseZoneName": "EU Commercial High",
                        "Description": "High density businesses that require mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_EducatedHighDensityCommercial": {
                "Name": "EU Educated High Density Business",
                    "BaseZoneName": "EU Commercial High",
                        "Description": "High density businesses that require mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_PoorlyEducatedHighDensityCommercial": {
                "Name": "NA Poorly Educated High Density Business",
                    "BaseZoneName": "NA Commercial High",
                        "Description": "High density businesses that require mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_EducatedHighDensityCommercial": {
                "Name": "NA Educated High Density Business",
                    "BaseZoneName": "NA Commercial High",
                        "Description": "High density businesses that require mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_PoorlyEducatedMixed": {
                "Name": "EU Poorly Educated Mixed Housing",
                    "BaseZoneName": "EU Residential Mixed",
                        "Description": "Mixed-use housing with commercial space that requires mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_EducatedMixed": {
                "Name": "EU Educated Mixed Housing",
                    "BaseZoneName": "EU Residential Mixed",
                        "Description": "Mixed-use housing with commercial space that requires mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_PoorlyEducatedMixed": {
                "Name": "NA Poorly Educated Mixed Housing",
                    "BaseZoneName": "NA Residential Mixed",
                        "Description": "Mixed-use housing with commercial space that requires mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_EducatedMixed": {
                "Name": "NA Educated Mixed Housing",
                    "BaseZoneName": "NA Residential Mixed",
                        "Description": "Mixed-use housing with commercial space that requires mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "UK_PoorlyEducatedMixed": {
                "Name": "UK Poorly Educated Mixed Housing",
                    "BaseZoneName": "UK Residential Mixed",
                        "Description": "Mixed-use housing with commercial space that requires mostly Poorly Educated workers at Level 1 and mostly Educated workers at Level 5 - Petrochemicals, Lodging, Chemicals, Plastics, Vehicles, Beverages, Convenience Food, Food, Textiles, Paper, Furniture, Meals, Entertainment, Recreation.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_food.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                                "Lodging",
                                "Chemicals",
                                "Plastics",
                                "Vehicles",
                                "Beverages",
                                "ConvenienceFood",
                                "Food",
                                "Textiles",
                                "Paper",
                                "Furniture",
                                "Meals",
                                "Entertainment",
                                "Recreation",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "UK_EducatedMixed": {
                "Name": "UK Educated Mixed Housing",
                    "BaseZoneName": "UK Residential Mixed",
                        "Description": "Mixed-use housing with commercial space that requires mostly Educated workers at Level 1 and mostly Well Educated workers at Level 5 - Pharmaceuticals, Electronics.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_electronics.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Pharmaceuticals",
                                "Electronics",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
        }
    }
    """;
    public static readonly string commercialByModel = """
    {
        "Version": "v1alpha1",
            "Zones": {
            "EU_GasStation": {
                "Name": "EU Gas Station",
                    "BaseZoneName": "EU Commercial Low",
                        "Description": "Low density gas stations - Petrochemicals.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_petrochemicals.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_GasStation": {
                "Name": "NA Gas Station",
                    "BaseZoneName": "NA Commercial Low",
                        "Description": "Low density gas stations - Petrochemicals.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_petrochemicals.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Petrochemicals",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_Motel": {
                "Name": "EU Motel",
                    "BaseZoneName": "EU Commercial Low",
                        "Description": "Low density motels - Lodging.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_lodging.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Lodging",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_Motel": {
                "Name": "NA Motel",
                    "BaseZoneName": "NA Commercial Low",
                        "Description": "Low density motels - Lodging.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_lodging.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Lodging",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "EU_Hotel": {
                "Name": "EU Hotel",
                    "BaseZoneName": "EU Commercial High",
                        "Description": "High density hotels - Lodging.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_lodging.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Lodging",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
            "NA_Hotel": {
                "Name": "NA Hotel",
                    "BaseZoneName": "NA Commercial High",
                        "Description": "High density hotels - Lodging.",
                            "Color": {
                    "r": 0.0,
                        "g": 0.0,
                            "b": 0.0,
                                "a": 1.0,
                                    "grayscale": 0.587,
                                        "maxColorComponent": 1.0
                },
                "IconUri": "coui://speciz/Resource_lodging.png",
                    "Filters": [
                        {
                            "ManufacturedResources": [],
                            "StoredResources": [],
                            "SoldResources": [
                                "Lodging",
                            ],
                            "RequireManufactured": null,
                            "RequireStored": null,
                            "RequireSold": null
                        }
                    ]
            },
        }
    }
    """;
}