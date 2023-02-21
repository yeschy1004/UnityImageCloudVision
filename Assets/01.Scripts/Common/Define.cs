namespace Yeschy1004
{
    public class Define
    {
		#region Google Cloud Vision API
		public enum FeatureType
		{
			TYPE_UNSPECIFIED,
			FACE_DETECTION,
			LANDMARK_DETECTION,
			LOGO_DETECTION,
			LABEL_DETECTION,
			TEXT_DETECTION,
			SAFE_SEARCH_DETECTION,
			IMAGE_PROPERTIES
		}

		public const int landmarkTypeNum = 34; //except unkown landmark
		public enum LandmarkType
		{
			UNKNOWN_LANDMARK,
			LEFT_EYE,
			RIGHT_EYE,
			LEFT_OF_LEFT_EYEBROW,
			RIGHT_OF_LEFT_EYEBROW,
			LEFT_OF_RIGHT_EYEBROW,
			RIGHT_OF_RIGHT_EYEBROW,
			MIDPOINT_BETWEEN_EYES,
			NOSE_TIP,
			UPPER_LIP,
			LOWER_LIP,
			MOUTH_LEFT,
			MOUTH_RIGHT,
			MOUTH_CENTER,
			NOSE_BOTTOM_RIGHT,
			NOSE_BOTTOM_LEFT,
			NOSE_BOTTOM_CENTER,
			LEFT_EYE_TOP_BOUNDARY,
			LEFT_EYE_RIGHT_CORNER,
			LEFT_EYE_BOTTOM_BOUNDARY,
			LEFT_EYE_LEFT_CORNER,
			RIGHT_EYE_TOP_BOUNDARY,
			RIGHT_EYE_RIGHT_CORNER,
			RIGHT_EYE_BOTTOM_BOUNDARY,
			RIGHT_EYE_LEFT_CORNER,
			LEFT_EYEBROW_UPPER_MIDPOINT,
			RIGHT_EYEBROW_UPPER_MIDPOINT,
			LEFT_EAR_TRAGION,
			RIGHT_EAR_TRAGION,
			LEFT_EYE_PUPIL,
			RIGHT_EYE_PUPIL,
			FOREHEAD_GLABELLA,
			CHIN_GNATHION,
			CHIN_LEFT_GONION,
			CHIN_RIGHT_GONION
		};

		public enum Likelihood
		{
			UNKNOWN,
			VERY_UNLIKELY,
			UNLIKELY,
			POSSIBLE,
			LIKELY,
			VERY_LIKELY
		}
		#endregion 

		#region 
		public const int safeSearchAnnotationsNum = 5;
		public enum SafeSearchAnnotations
        {
			adult = 0,
			spoof,
			medical,
			violence,
			racy
		}
		#endregion

	}

	public class InternalText
    {
		public const string keyworldList = "Keywords List . . .";
		public const string analysisStart = "Analyzing . . .";
		public const string nothingMatch = "Nothing Match";
		public const string nullAPIResponse = "{}";
		public const string nullInput = "[ERROR] Null Input";
		public const string type = "Type: ";
	}

	public class JsonResponses
    {
		public const int responses = 0;
		public const int description = 0;
		public const int score = 0;
		public const int labelAnnotations = 0;
		public const int landmarkAnnotations = 0;
		public const int textAnnotations = 0;
		public const int safeSearchAnnotation = 0;
		public const int logoAnnotations = 0;
		public const int faceAnnotations = 0;
		public const int imagePropertiesAnnotation = 0;
		public const int dominantColors = 0;
		public const int colors = 0;
		public const int color = 0;
		public const int red = 0;
		public const int green = 0;
		public const int blue = 0;
	}
}

