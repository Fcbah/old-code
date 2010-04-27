//
// libexif-wrapper.cs : LibExif wrapper for Mphoto to use
//
// Author:
//   Ravi Pratap     (ravi@ximian.com)
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) 2002 Ximian, Inc.
//

// Until the mono runtime bug is fixed
//#define NO_EXIF

using System;
using System.Collections;
using System.Runtime.InteropServices;

public enum ExifTag {
	InteroperabilityIndex		= 0x0001,
	InteroperabilityVersion	        = 0x0002,
	ImageWidth 			= 0x0100,
	ImageLength 			= 0x0101,
	BitsPersample 	         	= 0x0102,
	Compression 			= 0x0103,
	PhotometricInterpretation 	= 0x0106,
	FillOrder 			= 0x010a,
	DocumentName 			= 0x010d,
	ImageDescription 		= 0x010e,
	Make 				= 0x010f,
	Model 				= 0x0110,
	StripOffsets 			= 0x0111,
	Orientation 			= 0x0112,
	SamplesPerPixel 		= 0x0115,
	RowsPerStrip    		= 0x0116,
	StripByteCounts 		= 0x0117,
	XResolution 			= 0x011a,
	YResolution 			= 0x011b,
	PlanarConfiguration 		= 0x011c,
	ResolutionUnit  		= 0x0128,
	TransferFunction 		= 0x012d,
	Software 			= 0x0131,
	DateTime			= 0x0132,
	Artist				= 0x013b,
	WhitePoint			= 0x013e,
	PrimaryChromaticities		= 0x013f,
	TransferRange			= 0x0156,
	JPEGProc			= 0x0200,
	JPEGInterchangeFormat	        = 0x0201,
	JPEGInterchangeFormatLength	= 0x0202,
	YCBCRCoefficients		= 0x0211,
	YCBCRSubSampling		= 0x0212,
	YCBCRPositioning		= 0x0213,
	ReferenceBlackWhite		= 0x0214,
	RelatedImageFileFormat   	= 0x1000,
	RelatedImageWidth		= 0x1001,
	RelatedImageLength		= 0x1002,
	CFARepeatPatternDim		= 0x828d,
	CFAPattern			= 0x828e,
	BatteryLevel			= 0x828f,
	Copyright			= 0x8298,
	ExposureTime			= 0x829a,
	FNumber 			= 0x829d,
	IPTCNAA	        		= 0x83bb,
	IfdPointer      		= 0x8769,
	InterColorProfile		= 0x8773,
	ExposureProgram 		= 0x8822,
	SpectralSensitivity		= 0x8824,
	GPSInfoIfdPointer		= 0x8825,
	ISOSpeedRatings	        	= 0x8827,
	OECF				= 0x8828,
	ExifVersion			= 0x9000,
	DateTimeOriginal		= 0x9003,
	DateTimeDigitized		= 0x9004,
	ComponentsConfiguration	        = 0x9101,
	CompressedBitsPerPixel	        = 0x9102,
	ShutterSpeedValue		= 0x9201,
	ApertureValue			= 0x9202,
	BrightnessValue  		= 0x9203,
	ExposureBiasValue		= 0x9204,
	MaxApertureValue		= 0x9205,
	SubjectDistance 		= 0x9206,
	MeteringMode			= 0x9207,
	LightSource			= 0x9208,
	Flash				= 0x9209,
	FocalLength			= 0x920a,
	SubjectArea			= 0x9214,
	MakerNote			= 0x927c,
	UserComment			= 0x9286,
	SubSecTime			= 0x9290,
	SubSecTimeOriginal		= 0x9291,
	SubSecTimeDigitized		= 0x9292,
	FlashPixVersion 		= 0xa000,
	ColorSpace			= 0xa001,
	PixelXDimension 		= 0xa002,
	PixelYDimension 		= 0xa003,
	RelatedSoundFile		= 0xa004,
	InteroperabilityIfdPointer	= 0xa005,
	FlashEnergy			= 0xa20b,
	SpatialFrequencyResponse	= 0xa20c,
	FocalPlaneXResolution	        = 0xa20e,
	FocalPlaneYResolution	        = 0xa20f,
	FocalPlaneResolutionUnit	= 0xa210,
	SubjectLocation 		= 0xa214,
	ExposureIndex			= 0xa215,
	SensingMethod			= 0xa217,
	FileSource			= 0xa300,
	SceneType			= 0xa301,
	NewCFAPattern		        = 0xa302,
	CustomRendered  		= 0xa401,
	ExposureMode			= 0xa402,
	WhiteBalance			= 0xa403,
	DigitalZoomRatio		= 0xa404,
	FocalLengthIn35mmFilm	        = 0xa405,
	SceneCaptureType		= 0xa406,
	GainControl			= 0xa407,
	Contrast			= 0xa408,
	Saturation			= 0xa409,
	Sharpness			= 0xa40a,
	DeviceSettingDescription	= 0xa40b,
	SubjectDistanceRange		= 0xa40c,
	ImageUniqueId   		= 0xa420
}

public enum ExifByteOrder {
	Motorola,
	Intel
}

public enum ExifFormat {
	Byte      = 1,
	Ascii     = 2,
	Short     = 3,
	Long      = 4,
	Rational  = 5,
	Undefined = 7,
	Slong     = 9,
	SRational = 10
}

public enum ExifIfd {
	Zero = 0,
	One,
	Exif,
	Gps,
	InterOperability,
	Count
}


#if NO_EXIF
internal class ExifUtil {
	public static string GetTagName (ExifTag tag)
	{
		return "";
	}

	public static string GetTagTitle (ExifTag tag)
	{
		return "";
	}

	public static string GetTagDescription (ExifTag tag)
	{
		return "";
	}

	public static string GetByteOrderName (ExifByteOrder order)
	{
		return "";
	}

	public static string GetFormatName (ExifFormat format)
	{
		return "";
	}

	public static char GetFormatSize (ExifFormat format)
	{
		return ' ';
	}

	public static string GetIfdName (ExifIfd ifd)
	{
		return "";
	}
	
}

#else

internal class ExifUtil {
		
	[DllImport ("libexif.so")]
	static extern string exif_tag_get_name (ExifTag tag);

	[DllImport ("libexif.so")]
	static extern string exif_tag_get_title (ExifTag tag);

	[DllImport ("libexif.so")]
	static extern string exif_tag_get_description (ExifTag tag);

	[DllImport ("libexif.so")]
	static extern string exif_byte_order_get_name (ExifByteOrder order);

	[DllImport ("libexif.so")]
	static extern string exif_format_get_name (ExifFormat format);

	[DllImport ("libexif.so")]
	static extern char exif_format_get_size (ExifFormat format);

	[DllImport ("libexif.so")]
	static extern string exif_ifd_get_name (ExifIfd ifd);


	public static string GetTagName (ExifTag tag)
	{
		return exif_tag_get_name (tag);
	}

	public static string GetTagTitle (ExifTag tag)
	{
		return exif_tag_get_title (tag);
	}

	public static string GetTagDescription (ExifTag tag)
	{
		return exif_tag_get_description (tag);
	}

	public static string GetByteOrderName (ExifByteOrder order)
	{
		return exif_byte_order_get_name (order);
	}

	public static string GetFormatName (ExifFormat format)
	{
		return exif_format_get_name (format);
	}

	public static char GetFormatSize (ExifFormat format)
	{
		return exif_format_get_size (format);
	}

	public static string GetIfdName (ExifIfd ifd)
	{
		return exif_ifd_get_name (ifd);
	}
	
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct _ExifContent {
	_ExifEntry *entries;
	uint count;
	_ExifData *parent;

	[DllImport ("libexif.so")]
	internal static extern IntPtr exif_content_get_entry (_ExifContent *ptr, ExifTag tag);

	[DllImport ("libexif.so")]
	internal static unsafe extern IntPtr exif_content_foreach_entry (_ExifContent *ptr,
									 ExifContentForeachEntryFunc func,
									 void *data);
	
	internal delegate void ExifContentForeachEntryFunc (_ExifEntry *e, void *data);
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct _ExifEntry {
	readonly public ExifTag tag;
	readonly public int format;
	readonly public ulong components;
	readonly public byte *data;
	readonly public uint  size;
	readonly public _ExifContent *parent;

	[DllImport ("libexif.so")]
	internal static extern string exif_entry_get_value (_ExifEntry *entry);

	[DllImport ("libexif.so")]
	internal static extern string exif_entry_get_value_brief (_ExifEntry *entry);
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct _ExifData {
	_ExifContent *ifd0;
	_ExifContent *ifd1;
	_ExifContent *ifd_exif;
	_ExifContent *ifd_gps;
	_ExifContent *ifd_interop;
	internal IntPtr  data;
	internal int      size;

	[DllImport ("libexif.so")]
	internal static extern _ExifData *exif_data_new_from_file (string path);

	[DllImport ("libexif.so")]
	internal static extern _ExifData *exif_data_new_from_data (string data, uint size);

	[DllImport ("libexif.so")]
	internal static extern void exif_data_ref (_ExifData *data);

	[DllImport ("libexif.so")]
	internal static extern void exif_data_unref (_ExifData *data);

	[DllImport ("libexif.so")]
	internal static extern void exif_data_free (_ExifData _data);

	[DllImport ("libexif.so")]
	static extern void exif_data_dump (_ExifData *data);

	internal delegate void ExifDataForeachContentFunc (_ExifContent *content, void *user_data);

	[DllImport ("libexif.so")]
	internal unsafe static extern void exif_data_foreach_content(_ExifData *data, ExifDataForeachContentFunc func, void *ptr);
}

#endif

public class ExifData : IDisposable {
#if NO_EXIF
	public ExifData (string filename) {}
	public ExifData (string data, uint size) {}
	public void Dispose () {}
	~ExifData() {}
	public void Assemble () {}
	public string Lookup (ExifTag tag) { return null; }
	public Hashtable Values {
		get {
			return null;
		}
	}
	public byte[] Data {
		get {
			return null;
		}
	}
#else
	unsafe _ExifData *obj;
	
	public ExifData (string filename)
	{
		unsafe {
			obj = _ExifData.exif_data_new_from_file (filename);
		}
	}

	public ExifData (string data, uint size)
	{
		unsafe {
			obj = _ExifData.exif_data_new_from_data (data, size);
		}
	}

	public void Dispose ()
	{
		Dispose (true);
		GC.SuppressFinalize (this);
	}

	~ExifData ()
	{
		Dispose (false);
	}

	_ExifContent.ExifContentForeachEntryFunc content_func;
	
	unsafe void Assemble (_ExifContent *content, void *user_data)
	{
		_ExifContent.exif_content_foreach_entry (content, content_func, null);
	}

	unsafe void AssembleContent (_ExifEntry *entry, void *data)
	{
		values [entry->tag] = _ExifEntry.exif_entry_get_value (entry);
	}
	
	public void Assemble ()
	{
		unsafe {
			content_func = new _ExifContent.ExifContentForeachEntryFunc (AssembleContent);

			values = new Hashtable ();
			_ExifData.exif_data_foreach_content (obj, new _ExifData.ExifDataForeachContentFunc (Assemble), null);
		}
	}
	
	protected virtual void Dispose (bool disposing)
	{
		unsafe {
			if (obj != null){
				_ExifData.exif_data_unref (obj);
				obj = null;
			}
		}
		values = null;
	}

	Hashtable values;

	public string Lookup (ExifTag tag)
	{
		if (values == null)
			Assemble ();
		return (string) values [tag];
	}

	public Hashtable Values {
		get {
			return values;
		}
	}

	unsafe internal _ExifEntry * Search (ExifTag tag)
	{
		return null;
	}

	byte [] empty = new byte [0];
	
	public byte [] Data {
		get {
			unsafe {
				byte [] result;

				if (obj == null || obj->data == (IntPtr) 0)
					result = empty;
				else {
					result = new byte [obj->size];
					Marshal.Copy (obj->data, result, 0, obj->size);

				}
				return result;
			}
		}
	}
#endif
}

#if NO_EXIF
#else
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct _ExifNote {
	[DllImport ("libexif.so")]
	internal static extern void exif_note_ref (_ExifNote *note);

	[DllImport ("libexif.so")]
	internal static extern void exif_note_unref (_ExifNote *note);

	[DllImport ("libexif.so")]
	internal static extern void exif_note_free (_ExifNote *note);

	[DllImport ("libexif.so")]
	internal static extern IntPtr exif_note_new_from_data (string data, uint size);
	
	[DllImport ("libexif.so")]
	internal static extern string [] exif_note_get_value (_ExifNote *note);

	[DllImport ("libexif.so")]
	internal static extern void exif_note_set_byte_order (_ExifNote *note, ExifByteOrder order);

	[DllImport ("libexif.so")]
	internal static extern ExifByteOrder exif_note_get_byte_order (_ExifNote *note);
}

#endif
