using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Once a user clicks on the profile image, particular class gets called.
public class EditProfilePicture : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject profileGalleryPanel;
    // [SerializeField] private GameObject profilePicture;

    [SerializeField] private GameObject parentPanel;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[Edit Profile Picture] Button Click on Profile Picture");
        profileGalleryPanel.SetActive(true);
        parentPanel.SetActive(false);
    }

}