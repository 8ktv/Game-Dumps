using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Android;

[StructLayout(LayoutKind.Sequential)]
[NativeAsStruct]
[NativeType(Header = "Modules/AndroidJNI/Public/AndroidConfiguration.bindings.h")]
[RequiredByNativeCode]
public sealed class AndroidConfiguration
{
	private const int UiModeNightMask = 48;

	private const int UiModeTypeMask = 15;

	private const int ScreenLayoutDirectionMask = 192;

	private const int ScreenLayoutLongMask = 48;

	private const int ScreenLayoutRoundMask = 768;

	private const int ScreenLayoutSizeMask = 15;

	private const int ColorModeHdrMask = 12;

	private const int ColorModeWideColorGamutMask = 3;

	private int colorMode { get; set; }

	public int densityDpi { get; private set; }

	public float fontScale { get; private set; }

	public int fontWeightAdjustment { get; private set; }

	public AndroidKeyboard keyboard { get; private set; }

	public AndroidHardwareKeyboardHidden hardKeyboardHidden { get; private set; }

	public AndroidKeyboardHidden keyboardHidden { get; private set; }

	public int mobileCountryCode { get; private set; }

	public int mobileNetworkCode { get; private set; }

	public AndroidNavigation navigation { get; private set; }

	public AndroidNavigationHidden navigationHidden { get; private set; }

	public AndroidOrientation orientation { get; private set; }

	public int screenHeightDp { get; private set; }

	public int screenWidthDp { get; private set; }

	public int smallestScreenWidthDp { get; private set; }

	private int screenLayout { get; set; }

	public AndroidTouchScreen touchScreen { get; private set; }

	private int uiMode { get; set; }

	private string primaryLocaleCountry { get; set; }

	private string primaryLocaleLanguage { get; set; }

	public AndroidLocale[] locales
	{
		get
		{
			if (primaryLocaleCountry == null && primaryLocaleLanguage == null)
			{
				return new AndroidLocale[0];
			}
			return new AndroidLocale[1]
			{
				new AndroidLocale(primaryLocaleCountry, primaryLocaleLanguage)
			};
		}
	}

	public AndroidColorModeHdr colorModeHdr => (AndroidColorModeHdr)(colorMode & 0xC);

	public AndroidColorModeWideColorGamut colorModeWideColorGamut => (AndroidColorModeWideColorGamut)(colorMode & 3);

	public AndroidScreenLayoutDirection screenLayoutDirection => (AndroidScreenLayoutDirection)(screenLayout & 0xC0);

	public AndroidScreenLayoutLong screenLayoutLong => (AndroidScreenLayoutLong)(screenLayout & 0x30);

	public AndroidScreenLayoutRound screenLayoutRound => (AndroidScreenLayoutRound)(screenLayout & 0x300);

	public AndroidScreenLayoutSize screenLayoutSize => (AndroidScreenLayoutSize)(screenLayout & 0xF);

	public AndroidUIModeNight uiModeNight => (AndroidUIModeNight)(uiMode & 0x30);

	public AndroidUIModeType uiModeType => (AndroidUIModeType)(uiMode & 0xF);

	public AndroidConfiguration()
	{
	}

	public AndroidConfiguration(AndroidConfiguration otherConfiguration)
	{
		CopyFrom(otherConfiguration);
	}

	public void CopyFrom(AndroidConfiguration otherConfiguration)
	{
		colorMode = otherConfiguration.colorMode;
		densityDpi = otherConfiguration.densityDpi;
		fontScale = otherConfiguration.fontScale;
		fontWeightAdjustment = otherConfiguration.fontWeightAdjustment;
		keyboard = otherConfiguration.keyboard;
		hardKeyboardHidden = otherConfiguration.hardKeyboardHidden;
		keyboardHidden = otherConfiguration.keyboardHidden;
		mobileCountryCode = otherConfiguration.mobileCountryCode;
		mobileNetworkCode = otherConfiguration.mobileNetworkCode;
		navigation = otherConfiguration.navigation;
		navigationHidden = otherConfiguration.navigationHidden;
		orientation = otherConfiguration.orientation;
		screenHeightDp = otherConfiguration.screenHeightDp;
		screenWidthDp = otherConfiguration.screenWidthDp;
		smallestScreenWidthDp = otherConfiguration.smallestScreenWidthDp;
		screenLayout = otherConfiguration.screenLayout;
		touchScreen = otherConfiguration.touchScreen;
		uiMode = otherConfiguration.uiMode;
		primaryLocaleCountry = otherConfiguration.primaryLocaleCountry;
		primaryLocaleLanguage = otherConfiguration.primaryLocaleLanguage;
	}

	[Preserve]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"* ColorMode, Hdr: {colorModeHdr}");
		stringBuilder.AppendLine($"* ColorMode, Gamut: {colorModeWideColorGamut}");
		stringBuilder.AppendLine($"* DensityDpi: {densityDpi}");
		stringBuilder.AppendLine($"* FontScale: {fontScale}");
		stringBuilder.AppendLine($"* FontWeightAdj: {fontWeightAdjustment}");
		stringBuilder.AppendLine($"* Keyboard: {keyboard}");
		stringBuilder.AppendLine($"* Keyboard Hidden, Hard: {hardKeyboardHidden}");
		stringBuilder.AppendLine($"* Keyboard Hidden, Normal: {keyboardHidden}");
		stringBuilder.AppendLine($"* Mcc: {mobileCountryCode}");
		stringBuilder.AppendLine($"* Mnc: {mobileNetworkCode}");
		stringBuilder.AppendLine($"* Navigation: {navigation}");
		stringBuilder.AppendLine($"* NavigationHidden: {navigationHidden}");
		stringBuilder.AppendLine($"* Orientation: {orientation}");
		stringBuilder.AppendLine($"* ScreenHeightDp: {screenHeightDp}");
		stringBuilder.AppendLine($"* ScreenWidthDp: {screenWidthDp}");
		stringBuilder.AppendLine($"* SmallestScreenWidthDp: {smallestScreenWidthDp}");
		stringBuilder.AppendLine($"* ScreenLayout, Direction: {screenLayoutDirection}");
		stringBuilder.AppendLine($"* ScreenLayout, Size: {screenLayoutSize}");
		stringBuilder.AppendLine($"* ScreenLayout, Long: {screenLayoutLong}");
		stringBuilder.AppendLine($"* ScreenLayout, Round: {screenLayoutRound}");
		stringBuilder.AppendLine($"* TouchScreen: {touchScreen}");
		stringBuilder.AppendLine($"* UiMode, Night: {uiModeNight}");
		stringBuilder.AppendLine($"* UiMode, Type: {uiModeType}");
		stringBuilder.AppendLine($"* Locales ({locales.Length}):");
		for (int i = 0; i < locales.Length; i++)
		{
			AndroidLocale androidLocale = locales[i];
			stringBuilder.AppendLine($"* Locale[{i}] {androidLocale.country}-{androidLocale.language}");
		}
		return stringBuilder.ToString();
	}
}
