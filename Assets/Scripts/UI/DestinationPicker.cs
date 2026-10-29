using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DestinationPicker : MonoBehaviour
{
    static readonly Color Normal = new Color(0.05f, 0.12f, 0.16f, 0.86f);
    static readonly Color Selected = new Color(0f, 0.45f, 0.55f, 0.94f);

    [SerializeField] MapManager map;
    [SerializeField] NavigationController navigation;
    [SerializeField] RectTransform listRoot;
    [SerializeField] Button template;

    Image selectedImage;

    void Start()
    {
        Fill();
    }

    void Fill()
    {
        if (map == null || navigation == null || listRoot == null || template == null)
            return;

        template.gameObject.SetActive(false);

        IReadOnlyList<NavigationNode> destinations = map.Destinations;
        for (int i = 0; i < destinations.Count; i++)
        {
            NavigationNode node = destinations[i];
            if (node == null || !node.isDestination || string.IsNullOrEmpty(node.id))
                continue;

            Button button = Instantiate(template, listRoot);
            button.gameObject.SetActive(true);
            button.name = node.id;

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = node.name;

            string id = node.id;
            RoomPress press = button.gameObject.AddComponent<RoomPress>();
            press.Bind(this, id);
        }
    }

    public void Pick(Button button, string id)
    {
        if (selectedImage != null)
            selectedImage.color = Normal;

        if (button.targetGraphic is Image image)
        {
            image.color = Selected;
            selectedImage = image;
        }

        if (navigation != null)
            navigation.SetDestination(id);
    }
}

public class RoomPress : MonoBehaviour, IPointerDownHandler
{
    DestinationPicker picker;
    string id;

    public void Bind(DestinationPicker owner, string nodeId)
    {
        picker = owner;
        id = nodeId;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (picker != null)
            picker.Pick(GetComponent<Button>(), id);
    }
}
