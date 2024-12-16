using UnityEngine;

public class enemy : MonoBehaviour
{
    public int zaladowanaKarta = 0;
    public int hp = 0;
    public int atak = 0;
    public bool alive = false;
    public bool updateNeeded = false;
    public Skrypt gracz;
    Ray ray;
	RaycastHit hit;
    GameObject pionek, HP, ATT, figurki;
    MeshFilter mesh1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pionek = gameObject.transform.GetChild(0).gameObject;
        figurki = GameObject.Find("Figurki");
        mesh1 = pionek.GetComponent<MeshFilter>();
        pionek.transform.position = gameObject.transform.position + new Vector3(0,0,-10000); //zejdź na dół
        hp = 0;
        atak = 0;
        HP = gameObject.transform.GetChild(1).gameObject;
        ATT = gameObject.transform.GetChild(2).gameObject;
        resetText();
    }

    void updateText(){
        HP.GetComponent<TextMesh>().text = hp.ToString();
        ATT.GetComponent<TextMesh>().text = atak.ToString();
        var mesh2 = figurki.transform.GetChild(zaladowanaKarta-1).gameObject.GetComponent<MeshFilter>();
        mesh1.mesh = mesh2.mesh;

    }
    void resetText(){
        HP.GetComponent<TextMesh>().text = "HP";
        ATT.GetComponent<TextMesh>().text = "ATT";
    }

    // Update is called once per frame
    void Update()
    {
        if(updateNeeded) {
            updateText();
            updateNeeded = false;
        }
        if(dane.cofnieciaWroga > 0 && dane.enemyPole[0] == gameObject.name){ //jesli są akcje do cofniecia i nazwa tego pola jest na liscie to cofnij
            switch (dane.enemyAkcje[0]){
                case dane.action.LoadCard: //usuwamy kartę
                    zaladowanaKarta = 0;
                    alive = false;
                    hp = 0;
                    atak = 0;
                    dane.energiaWroga += dane.meWartosc[0];
                    if(dane.energiaWroga > 5) dane.energiaWroga = 5;
                    pionek.transform.position = gameObject.transform.position + new Vector3(0,0,-10000);
                    resetText();
                break;
                case dane.action.Die: //reanimacja
                    alive = true;
                    hp = dane.enemyWartosc[0];
                    pionek.transform.position = gameObject.transform.position + new Vector3(0,0,100);
                    updateText();
                break;
                case dane.action.LoseHP: //uzdrowienie
                    hp = hp + dane.enemyWartosc[0];
                    updateText();
                break;
                case dane.action.Null:
                    //nic XD skill issue
                break;
            }
            dane.UnoReverse(false);
        }
        gameObject.GetComponent<Renderer>().material.color = Color.black; // domyślny kolor
        if(dane.pole == gameObject) gameObject.GetComponent<Renderer>().material.color = Color.green; //kolor jesli wybrane
        if(alive && hp <= 0){ //jeśli żywe i hp <= 0 to umżyj
            alive = false;
            pionek.transform.position = gameObject.transform.position + new Vector3(0,0,-10000); //zejdź na dół
            resetText();
        }
        if(!alive && hp > 0){ //jeśli martwy i hp > 0 to ożyj
            alive = true;
            pionek.transform.position = gameObject.transform.position + new Vector3(0,0,100); //wejdź na górę
            updateText();
        }


        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)) //czy myszka na coś wskazuje
		{
            if(gameObject.name == hit.collider.name){ //czy wskazuje na to pole
                if(Input.GetMouseButtonDown(0)){ //czy jest kliklięty lewy przycisk
                    if(dane.karta != null && dane.numerKarty < 0) { //karta wybrana i jest cofająca
                        dane.energiaGracza += dane.numerKarty;
                        dane.cofnieciaWroga += dane.losujCofniecie(dane.numerKarty);//cofanie
                        dane.karta = null;
                        dane.correctUnoReverse();
                        dane.wrogRusza = true;
                    }
                    else if(dane.pole != null && dane.pole.GetComponent<Skrypt>().alive && alive){   //jeśli pole gracza jest wybrane i żywe i to pole jest żywe to walka
                        if(dane.pole.GetComponent<Skrypt>().atak >= hp) dane.AddMove(dane.action.Die, gameObject.name, hp, false); //jeśli za mało życia, to dodaj ruch Die
                        else dane.AddMove(dane.action.LoseHP, gameObject.name, dane.pole.GetComponent<Skrypt>().atak, false); //inaczej LoseHealth
                        hp = hp - dane.pole.GetComponent<Skrypt>().atak; //zmniejsz życie
                        dane.silaAtakuWroga = atak; //przekaz siłe ataku
                        dane.ruchAtak = true; //ustaw flage ataku
                        updateText(); //aktualizuj tekst tego pionka
                        dane.wrogRusza = true; //ustaw kolejkę wroga
                    }  
                }
                gameObject.GetComponent<Renderer>().material.color = Color.blue; // kolor jesli myszka nad
            }
            else{

            }
                
		}
        

    }
}
