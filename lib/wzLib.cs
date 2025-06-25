using WzComparerR2.WzLib;

public static partial class WzLib
{
	public static Wz_Structure wzs = new();

	static WzLib() {
		wzs.WzVersionVerifyMode = WzVersionVerifyMode.Fast;
        string baseWz = @".\wzfiles\Base.wz";
		wzs.Load(baseWz, true);
    }
}
