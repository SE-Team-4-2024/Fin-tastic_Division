


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ProfileImagePrefabHandler : MonoBehaviour, IPointerClickHandler
{

    public GameObject tickMarkPrefab;
    public Transform tickMarksParent;
    public Transform parentPanel;

    private Image imageComponent;


    void Start(){
        generateDefaultSelection();
    }


    private void generateDefaultSelection()
{
    UserManager userManagerInstance = FindObjectOfType<UserManager>();
    Image[] images = parentPanel.GetComponentsInChildren<Image>();
    int defaultUserImage = userManagerInstance.GetImage();
    foreach (Image image in images)
    {
        // Check if the image name matches the default image name
        if (userManagerInstance.GetImageNo(image.name) == defaultUserImage)
        {
           
            // Instantiate a background image on the default image
            DeactivateBgImage();
            Vector3 imageCenter = image.transform.position;
            GameObject bgImageInstance = Instantiate(tickMarkPrefab, tickMarksParent);
            bgImageInstance.SetActive(true);
            bgImageInstance.transform.position = imageCenter;
            bgImageInstance.tag = "spawnedPrefabTag";
            
        }
    }
}

    public void OnPointerClick(PointerEventData eventData)
    {
        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        Image clickedImage = clickedObject.GetComponent<Image>();

        if (clickedImage != null && userManagerInstance.MatchString(clickedImage.name))
        {
            Debug.Log("Image clicked: " + clickedImage.name);
            userManagerInstance.SetImage(userManagerInstance.GetImageNo(clickedImage.name));
            // int imageNo = userManagerInstance.GetImageNo(clickedImage.name);
            // Deactivate the active previous selection (if any)
            DeactivateBgImage();
            // Instantiate a background image on the clicked image
            Vector3 imageCenter = clickedImage.transform.position;
            GameObject bgImageInstance = Instantiate(tickMarkPrefab, tickMarksParent);
            bgImageInstance.SetActive(true);
            bgImageInstance.transform.position = imageCenter;
            bgImageInstance.tag = "spawnedPrefabTag";
        }
        else
        {
            Debug.Log("Clicked object is not an image.");
        }
    }

    // Function to deactivate the active background image
    void DeactivateBgImage()
    {
        foreach (Transform child in parentPanel)
        {
            // Check if the child object is a prefab
            if (child.CompareTag("spawnedPrefabTag")) 
            {
                Destroy(child.gameObject);
            }
        }
    }
}
