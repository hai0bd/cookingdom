using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class CapyShadow : MonoBehaviour
    {
        [SerializeField] private CustomColorSkeleton customColor;
        
        // Start is called before the first frame update
        public void OnInit()
        {
            gameObject.SetActive(true);
            customColor.SetColor(Color.white);
            customColor.DoColor(Color.clear, 0.2f);
            Invoke(nameof(OnDespawn), 0.3f);
        }

        private void OnDespawn()
        {
            gameObject.SetActive(false);
        }
    }
}
