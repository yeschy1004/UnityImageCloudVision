using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Yeschy1004
{
	public class LoadGallery : MonoBehaviour
	{
		[Header("----------------UI Field---------------")]
		[SerializeField]
		private RawImage _img;
		[SerializeField]
		private Button _selectBtn;
		[SerializeField]
		private Button _analysisBtn;
		[SerializeField]
		private Text _responseText;
		private string _fileName;
		[SerializeField]
		private RectTransform _scrollContent;
	    [SerializeField]
		private GameObject _dominantColorObj;
		[SerializeField]
		private List<GameObject> _dominantColorList;
		

		[Header("-------------Setting Field-------------")]
		[SerializeField]
		private string _visionAPIurl = "https://vision.googleapis.com/v1/images:annotate?key=";
		// Quickstart: Set up the Vision API
		// https://cloud.google.com/vision/docs/setup?h&hl=en
		[SerializeField]
		private string _visionAPIKey = "KEY-VALUE";

		[SerializeField]
		private Define.FeatureType _featureType = Define.FeatureType.FACE_DETECTION;
		[SerializeField]
		private int _maxResults = 5;
		Dictionary<string, string> headers;
		private Texture2D texture2D;
		[SerializeField]
		private Text _selectedText;

		private List<string> _keywords;
		private List<string> _scores;

		[Header("-------------ValueSet Field------------")]
		private int _dataLimit = 50000000;
		private int _imgSize = 500;

		#region Settings
		[System.Serializable]
		public class AnnotateImageRequests
		{
			public List<AnnotateImageRequest> requests;
		}

		[System.Serializable]
		public class AnnotateImageRequest
		{
			public ImageRequest image;
			public List<Feature> features;
		}

		[System.Serializable]
		public class ImageRequest
		{
			public string content;
		}

		[System.Serializable]
		public class Feature
		{
			public string type;
			public int maxResults;
		}

		[System.Serializable]
		public class ImageContext
		{
			public LatLongRect latLongRect;
			public List<string> languageHints;
		}

		[System.Serializable]
		public class LatLongRect
		{
			public LatLng minLatLng;
			public LatLng maxLatLng;
		}

		[System.Serializable]
		public class AnnotateImageResponses
		{
			public List<AnnotateImageResponse> responses;
		}

		[System.Serializable]
		public class AnnotateImageResponse
		{
			public List<FaceAnnotation> faceAnnotations;
			public List<EntityAnnotation> landmarkAnnotations;
			public List<EntityAnnotation> logoAnnotations;
			public List<EntityAnnotation> labelAnnotations;
			public List<EntityAnnotation> textAnnotations;
		}

		[System.Serializable]
		public class FaceAnnotation
		{
			public BoundingPoly boundingPoly;
			public BoundingPoly fdBoundingPoly;
			public List<Landmark> landmarks;
			public float rollAngle;
			public float panAngle;
			public float tiltAngle;
			public float detectionConfidence;
			public float landmarkingConfidence;
			public string joyLikelihood;
			public string sorrowLikelihood;
			public string angerLikelihood;
			public string surpriseLikelihood;
			public string underExposedLikelihood;
			public string blurredLikelihood;
			public string headwearLikelihood;
		}

		[System.Serializable]
		public class EntityAnnotation
		{
			public string mid;
			public string locale;
			public string description;
			public float score;
			public float confidence;
			public float topicality;
			public BoundingPoly boundingPoly;
			public List<LocationInfo> locations;
			public List<Property> properties;
		}

		[System.Serializable]
		public class BoundingPoly
		{
			public List<Vertex> vertices;
		}

		[System.Serializable]
		public class Landmark
		{
			public string type;
			public Position position;
		}

		[System.Serializable]
		public class Position
		{
			public float x;
			public float y;
			public float z;
		}

		[System.Serializable]
		public class Vertex
		{
			public float x;
			public float y;
		}

		[System.Serializable]
		public class LocationInfo
		{
			LatLng latLng;
		}

		[System.Serializable]
		public class LatLng
		{
			float latitude;
			float longitude;
		}

		[System.Serializable]
		public class Property
		{
			string name;
			string value;
		}
		#endregion

		#region Unity MonoBehaviour Callbacks
		// Use this for initialization
		void Start()
		{
			_keywords = new List<string>(_maxResults);
			_scores = new List<string>(_maxResults);
			_dominantColorList = new List<GameObject>(_maxResults);

			for(int i=0; i<_maxResults; i++)
            {
				_dominantColorList.Add(Instantiate(_dominantColorObj, _scrollContent));
            }

			_selectedText.text = InternalText.type + _featureType.ToString();
			InitVisionAPI();

			_selectBtn?.onClick.AddListener(OnClickSelectBtn);
			_analysisBtn?.onClick.AddListener(OnClickAnalysisBtn);
		}
		#endregion

		#region Native Gallery
		private void OnClickSelectBtn()
		{
			NativeGallery.GetImageFromGallery((file) =>
			{
				if (string.IsNullOrEmpty(file))
				{
					Debug.Log(InternalText.nullInput);
					return;
				}

				FileInfo selected = new FileInfo(file);

				if (selected.Length > _dataLimit)
				{
					return;
				}

				StartCoroutine(LoadImage(file));
			});
		}

		IEnumerator LoadImage(string path)
		{
			yield return null;

			byte[] fileData = File.ReadAllBytes(path);
			string filename = Path.GetFileName(path).Split('.')[0];
			string savePath = Application.persistentDataPath + "/Image";

			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
			File.WriteAllBytes(savePath + filename + ".png", fileData);
			var temp = File.ReadAllBytes(savePath + filename + ".png");

			texture2D = new Texture2D(0, 0);
			texture2D.LoadImage(temp);

			_img.texture = texture2D;
			_img.SetNativeSize();
			ImageSizeSetting(_img, _imgSize, _imgSize);

		}

		private void ImageSizeSetting(RawImage img, float x, float y)
		{
			var imgX = img.rectTransform.sizeDelta.x;
			var imgY = img.rectTransform.sizeDelta.y;

			if (x / y > imgX / imgY)
			{
				img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
				img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imgX * (y / imgY));
			}
			else
			{
				img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
				img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imgY * (x / imgX));
			}
		}
		#endregion

		#region Request/Response API
		private void InitVisionAPI()
		{
			headers = new Dictionary<string, string>();
			headers.Add("Content-Type", "application/json; charset=UTF-8");

			if (string.IsNullOrEmpty(_visionAPIKey))
				Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");
		}

		public void SetFeatureType(Define.FeatureType _type)
		{
			_featureType = _type;
		}

		private void OnClickAnalysisBtn()
		{
			StartCoroutine(RequestAnalysis());
		}


		private IEnumerator RequestAnalysis()
		{
			Debug.Log(InternalText.analysisStart);
			SetResultText(InternalText.analysisStart);

			if (string.IsNullOrEmpty(_visionAPIKey))
			{
				yield return null;
				yield break;
			}

			Color[] pixels = texture2D.GetPixels();
			if (pixels.Length == 0)
			{
				yield return null;
				yield break;
			}

			texture2D.SetPixels(pixels);
			// texture2D.Apply(false); // Not required. Because we do not need to be uploaded it to GPU
			byte[] jpg = texture2D.EncodeToJPG();
			string base64 = System.Convert.ToBase64String(jpg);
#if UNITY_WEBGL
			Application.ExternalCall("post", this.gameObject.name, "OnSuccessFromBrowser", "OnErrorFromBrowser", this.url + this.apiKey, base64, this.featureType.ToString(), this.maxResults);
#else

			AnnotateImageRequests requests = new AnnotateImageRequests();
			requests.requests = new List<AnnotateImageRequest>();

			AnnotateImageRequest request = new AnnotateImageRequest();
			request.image = new ImageRequest();
			request.image.content = base64;
			request.features = new List<Feature>();

			Feature feature = new Feature();
			feature.type = this._featureType.ToString();
			feature.maxResults = this._maxResults;

			request.features.Add(feature);
			requests.requests.Add(request);

			string jsonData = JsonUtility.ToJson(requests, false);
			if (jsonData != string.Empty)
			{
				string url = this._visionAPIurl + this._visionAPIKey;
				byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
				using (WWW www = new WWW(url, postData, headers))
				{
					yield return www;
					if (string.IsNullOrEmpty(www.error))
					{
						Debug.Log(www.text.Replace("\n", "").Replace(" ", ""));
						AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.text);
						SetResultText(VisionAPIJsonParser(www.text));
						SampleOnAnnotateImageResponses(responses);
						yield return null;
						yield break;
					}
					else
					{
						Debug.Log("Error: " + www.error);
						Debug.Log("Error: " + www.url);
						SetResultText("Error: " + www.error);
						yield return null;
						yield break;
					}
				}
			}
#endif
		}

#if UNITY_WEBGL
	void OnSuccessFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
		AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(jsonString);
		Sample_OnAnnotateImageResponses(responses);
	}

	void OnErrorFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
	}
#endif

		/// <summary>
		/// A sample implementation.
		/// </summary>
		private void SampleOnAnnotateImageResponses(AnnotateImageResponses responses)
		{
			if (responses.responses.Count > 0)
			{
				if (responses.responses[0].faceAnnotations != null && responses.responses[0].faceAnnotations.Count > 0)
				{
					Debug.Log("joyLikelihood: " + responses.responses[0].faceAnnotations[0].joyLikelihood);
				}
			}
		}

		private void SetResultText(string result)
		{
			_responseText.text = result;
			LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollContent);
		}
		#endregion

		#region Json Parser

		private string VisionAPIJsonParser(string json)
		{
			string result = string.Empty;
			JObject jObject = JObject.Parse(json);
			SetDominantImg(false);
			_keywords = new List<string>(_maxResults);
			_scores = new List<string>(_maxResults);

			switch (_featureType)
			{
				case Define.FeatureType.LABEL_DETECTION:
					result = VisionAPILabelDetection(jObject);
					break;
				case Define.FeatureType.LANDMARK_DETECTION:
					result = VisionAPILandmrkDetection(jObject);
					break;
				case Define.FeatureType.TEXT_DETECTION:
					result = VisionAPITextDetection(jObject);
					break;
				case Define.FeatureType.TYPE_UNSPECIFIED:
					result = VisionAPITypeUnspecified(jObject);
					break;
				case Define.FeatureType.FACE_DETECTION:
					result = VisionAPIFaceDetection(jObject);
					break;
				case Define.FeatureType.LOGO_DETECTION:
					result = VisionAPILogoDetection(jObject);
					break;
				case Define.FeatureType.SAFE_SEARCH_DETECTION:
					result = VisionAPISafeSearchDetection(jObject);
					break;
				case Define.FeatureType.IMAGE_PROPERTIES:
					result = VisionAPIImageProperties(jObject);
					break;
			}
			return result;
		}

		private string VisionAPILabelDetection(JObject jObject)
		{
			StringBuilder sb = new StringBuilder();
		
			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			for (int i = 0; i < _maxResults; i++)
			{
				_keywords.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.labelAnnotations)]
					[i][nameof(JsonResponses.description)].ToString());
				_scores.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.labelAnnotations)]
					[i][nameof(JsonResponses.score)].ToString());

				sb.Append(_keywords[i]);
				sb.Append(": ");
				sb.Append((Math.Round(float.Parse(_scores[i]) * 100f, 2)));
				sb.Append("%\n\n");
			}

			return sb.ToString();
		}

		private string VisionAPILandmrkDetection(JObject jObject)
		{
			StringBuilder sb = new StringBuilder();

			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			_keywords.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.landmarkAnnotations)]
					[0][nameof(JsonResponses.description)].ToString());
			_scores.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.landmarkAnnotations)]
				[0][nameof(JsonResponses.score)].ToString());

			sb.Append(_keywords[0]);
			sb.Append(": ");
			sb.Append((Math.Round(float.Parse(_scores[0]) * 100f, 2)));
			sb.Append("%\n\n");

			return sb.ToString();
		}

		private string VisionAPITextDetection(JObject jObject)
		{
			StringBuilder sb = new StringBuilder();

			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			_keywords.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.textAnnotations)]
				[0][nameof(JsonResponses.description)].ToString());
			sb.Append(_keywords[0]);

			return sb.ToString();
		}

		private string VisionAPITypeUnspecified(JObject jObject)
		{
			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			return jObject.ToString();
		}

		private string VisionAPIFaceDetection(JObject jObject)
		{
			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			return jObject.ToString();
		}

		private string VisionAPILogoDetection(JObject jObject)
		{
			StringBuilder sb = new StringBuilder();

			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			_keywords.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.logoAnnotations)]
					[0][nameof(JsonResponses.description)].ToString());
			_scores.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.logoAnnotations)]
				[0][nameof(JsonResponses.score)].ToString());

			sb.Append(_keywords[0]);
			sb.Append(": ");
			sb.Append((Math.Round(float.Parse(_scores[0]) * 100f, 2)));
			sb.Append("%\n\n");

			return sb.ToString();
		}

		private string VisionAPISafeSearchDetection(JObject jObject)
		{
			StringBuilder sb = new StringBuilder();
			string safeSearchType;

			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			for (int i=0; i< Define.safeSearchAnnotationsNum; i++)
            {
				safeSearchType = ((Define.SafeSearchAnnotations)i).ToString();
				_keywords.Add(safeSearchType);
				_scores.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.safeSearchAnnotation)][safeSearchType].ToString());

				sb.Append(_keywords[i]);
				sb.Append(": ");
				sb.Append(_scores[i]);
				sb.Append("\n\n");
			}

			return sb.ToString();
		}

		private string VisionAPIImageProperties(JObject jObject)
		{
			StringBuilder sb = new StringBuilder();
			Color rgbCode;

			if (jObject[nameof(JsonResponses.responses)][0].ToString().Equals(InternalText.nullAPIResponse))
			{
				return InternalText.nothingMatch;
			}

			for (int i = 0; i < _maxResults; i++)
			{
				rgbCode = new Color(
					(int)jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.imagePropertiesAnnotation)][nameof(JsonResponses.dominantColors)]
					[nameof(JsonResponses.colors)][i][nameof(JsonResponses.color)][nameof(JsonResponses.red)] / 255f,
					(int)jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.imagePropertiesAnnotation)][nameof(JsonResponses.dominantColors)]
					[nameof(JsonResponses.colors)][i][nameof(JsonResponses.color)][nameof(JsonResponses.green)] / 255f,
					(int)jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.imagePropertiesAnnotation)][nameof(JsonResponses.dominantColors)]
					[nameof(JsonResponses.colors)][i][nameof(JsonResponses.color)][nameof(JsonResponses.blue)] / 255f);

				_keywords.Add(rgbCode.ToString());
				_scores.Add(jObject[nameof(JsonResponses.responses)][0][nameof(JsonResponses.imagePropertiesAnnotation)][nameof(JsonResponses.dominantColors)]
					[nameof(JsonResponses.colors)][i][nameof(JsonResponses.score)].ToString());

				_dominantColorList[i].SetActive(true);
				_dominantColorList[i].GetComponent<Image>().color = rgbCode;

				sb.Append(_keywords[i]);
				sb.Append(": ");
				sb.Append((Math.Round(float.Parse(_scores[i]) * 100f, 2)));
				sb.Append("%\n\n");
			}

			return sb.ToString();
		}

		private void SetDominantImg(bool isOn)
        {
			for (int i = 0; i < _maxResults; i++)
			{		
				_dominantColorList[i].SetActive(isOn);			
			}
		}

		public List<string> GetKeywords()
		{
			return _keywords;

		}

		#endregion
	}
}