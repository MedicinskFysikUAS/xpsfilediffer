enum TabType
{
    PROSTATE = 0,
    CYLINDER = 1,
    INTRAUTERINE = 2
}

public enum CylinderType
{
    VC = 0,
    SVC = 1
}

public enum XpsFileType
{
    ONCENTRA_PROSTATE_TREATMENT_PLAN = 0,
    ONCENTRA_PROSTATE_DVH = 1,
    PROSTATE_TCC = 2,
    ONCENTRA_CYLINDER_TREATMENT_PLAN = 3,
    CYLINDER_TCC = 4
    // ONCENTRA_INTRAUTERIN_TREATMENT_PLAN 
}


public static class Constants
{
    public const string DATE_AND_TIME_FORMAT = "yyyy-MM-dd HH:mm";
    public const decimal TIME_THRESHOLD = 0.05m;
}