using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yeschy1004
{
    public class GetFeatureType : MonoBehaviour
    {
        [SerializeField]
        private Define.FeatureType _curFeatureType;
        [SerializeField]
        private LoadGallery _loadGallery;
        [SerializeField]
        private Text _selectedText;

        private void Start()
        {
            this.GetComponent<Button>()?.onClick.AddListener(
                delegate
                {
                    _loadGallery.SetFeatureType(_curFeatureType);
                    _selectedText.text = InternalText.type + _curFeatureType.ToString();
                });
        }
    }
}