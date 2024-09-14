using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [HideInInspector]
    public List<Card> cardsInTable = new List<Card>();

    [SerializeField]
    private GameObject m1CardArrangement;

    [SerializeField]
    private GameObject m2CardArrangement;

    [SerializeField]
    private GameObject m3CardArrangement;

    [SerializeField]
    private GameObject m4CardArrangement;

    [SerializeField]
    private GameObject m5CardArrangement;

    public void Clear()
    {
        cardsInTable.Clear();
    }

    public void SetActiveCards(List<Card> pCards)
    {
        m1CardArrangement.SetActive(false);
        m2CardArrangement.SetActive(false);
        m3CardArrangement.SetActive(false);
        m4CardArrangement.SetActive(false);
        m5CardArrangement.SetActive(false);

        cardsInTable.Clear();

        GameObject activeLayout;
        switch (pCards.Count)
        {
            case 2:
            {
                activeLayout = m2CardArrangement;
                break;
            }
            case 3:
            {
                activeLayout = m3CardArrangement;
                break;
            }
            case 4:
            {
                activeLayout = m4CardArrangement;
                break;
            }
            case 5:
            {
                activeLayout = m5CardArrangement;
                break;
            }
            default:
            {
                activeLayout = m1CardArrangement;
                break;
            }
        }

        activeLayout.SetActive(true);
        Card[] cardsInLayout = activeLayout.GetComponentsInChildren<Card>();
        for(int i = 0; i < cardsInLayout.Length; i++)
        {
            SpriteRenderer renderer = cardsInLayout[i].GetComponentInChildren<SpriteRenderer>();
            renderer.sprite = ResourceManager.Instance.getSprite(pCards[i].cardName);
            cardsInTable.Add(pCards[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
