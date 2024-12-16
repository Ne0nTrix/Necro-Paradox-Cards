using UnityEngine;

public class karta : MonoBehaviour
{
    Ray ray;
	RaycastHit hit;
    public int rodzajKarty; //rodzaj karty, statystyki kart są w skrypt.cs, przypisywane dla danego obiektu w inspektorze unity
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rodzajKarty >= 1 && rodzajKarty <= 6 && dane.daneKart[rodzajKarty-1,2] > dane.energiaGracza ){ //jeśli rodzaj karty <1,6> i gracz ma za malo energii to wylacz karte
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            if(dane.karta == gameObject) dane.karta = null; //jeśli wybrana to odwybierz xd
            return;
        }
        if (rodzajKarty < 0 && -rodzajKarty > dane.energiaGracza ){ //jeśli rodzaj karty cofajacy i gracz ma za malo energii to wylacz karte
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            if(dane.karta == gameObject) dane.karta = null; //jeśli wybrana to odwybierz xd
            return;
        }
        if (rodzajKarty == 7 && dane.energiaGracza < 3){ //jeśli rodzaj karty <1,6> i gracz ma za malo energii to wylacz karte
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            if(dane.karta == gameObject) dane.karta = null; //jeśli wybrana to odwybierz xd
            return;
        } 
        gameObject.GetComponent<Renderer>().material.color = Color.white; //default kolor



        if (gameObject == dane.karta){
            gameObject.GetComponent<Renderer>().material.color = Color.green; //jeśli karta jest zaznaczona do kolor zielony
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)) //na co wskazuje myszka
		{
            if(gameObject.name == hit.collider.name){ //czy wskazuje na TĄ kartę


                if(Input.GetMouseButtonDown(0)){ //czy był kliknięty lewy przycisk
                    if(dane.pole == null){ //czy jest wybrane pole, jeśli nie to wybierz kartę
                        if(dane.karta == gameObject) { //jeśli ta karta jest zaznaczona do odznacz
                            dane.karta = null;
                            dane.numerKarty = 0;
                        }
                        else {
                            dane.karta = gameObject; //inaczej zaznacz
                            dane.numerKarty = rodzajKarty;
                        }
                    }
                    
                }
                gameObject.GetComponent<Renderer>().material.color = Color.blue; // jeśli nad jest myszka to kolor niebieski
            }
            else{
            }
                
		}
    }
}
