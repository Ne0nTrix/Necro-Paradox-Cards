using UnityEngine;
using System.Collections;

public class Skrypt : MonoBehaviour
{
    public int zaladowanaKarta = 0;
    public int hp = 0;
    public bool ruchWroga = true;
    public int atak = 0;
    public bool updateNeeded = false;
    public bool alive = false;
    Ray ray;
	RaycastHit hit;
    public GameObject pionek, HP, ATT, polaGracza, polaWroga, karty, figurki;
    MeshFilter mesh1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        polaGracza = GameObject.Find("polaGracza"); //ustawienie zmiennych
        polaWroga = GameObject.Find("polaWroga");
        karty = GameObject.Find("karty");
        figurki = GameObject.Find("Figurki");
        pionek = gameObject.transform.GetChild(0).gameObject;
        mesh1 = pionek.GetComponent<MeshFilter>();
        pionek.transform.position = gameObject.transform.position + new Vector3(0,0,-10000); //zejdź na dół
        hp= 0;
        atak = 0;
        HP = gameObject.transform.GetChild(1).gameObject;
        ATT = gameObject.transform.GetChild(2).gameObject;
        resetText();
    }

    void updateText(){ //aktualizowanie tekstu
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
        if(dane.wrogRusza) AI(); //jeśli wrog ma ruszyć to wywołaj AI
        if(updateNeeded) { //jeśli potrzeba update to go zrob
            updateText();
            updateNeeded = false;
        }

        if(dane.cofnieciaGracza > 0 && dane.mePole[0] == gameObject.name){ //jesli są akcje do cofniecia i nazwa tego pola jest na liscie to cofnij
            switch (dane.meAkcje[0]){
                case dane.action.LoadCard: //usuwamy kartę
                    zaladowanaKarta = 0;
                    alive = false;
                    hp = 0;
                    atak = 0;
                    dane.energiaGracza += dane.meWartosc[0]; //zwracamy energię
                    if(dane.energiaGracza > 5) dane.energiaGracza = 5;
                    pionek.transform.position = gameObject.transform.position + new Vector3(0,0,-10000);
                    resetText();
                break;
                case dane.action.Die: //reanimacja
                    alive = true;
                    hp = dane.meWartosc[0];
                    pionek.transform.position = gameObject.transform.position + new Vector3(0,0,100);
                    updateText();
                break;
                case dane.action.LoseHP: //uzdrowienie
                    hp = hp + dane.meWartosc[0];
                    updateText();
                break;
                case dane.action.Null:
                    //nic XD skill issue
                break;
            }
            dane.UnoReverse(true);
        }
        gameObject.GetComponent<Renderer>().material.color = Color.black; // domyślny kolor
        if(dane.ruchAtak && dane.pole == gameObject){ //atak
            if(dane.silaAtakuWroga >= hp) dane.AddMove(dane.action.Die, gameObject.name, hp, true);
            else dane.AddMove(dane.action.LoseHP, gameObject.name, dane.silaAtakuWroga, true);
            hp = hp - dane.silaAtakuWroga;
            dane.ruchAtak = false;
            dane.pole = null;
            updateText();
        }
        if(dane.pole == gameObject) gameObject.GetComponent<Renderer>().material.color = Color.green; //kolor jesli wybrane

        

        if(alive && hp <= 0){ //jeśli żywe i hp <= 0 to umżyj
            alive = false;
            resetText();
            pionek.transform.position = gameObject.transform.position + new Vector3(0,0,-10000); //zejdź na dół
        }
        if(!alive && hp > 0){ //jeśli martwy i hp > 0 to ożyj
            alive = true;
            pionek.transform.position = gameObject.transform.position + new Vector3(0,0,100); //wejdź na górę
        }


        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)) //czy myszka na coś wskazuje
		{
            if(gameObject.name == hit.collider.name){ //czy wskazuje na to pole
                if(Input.GetMouseButtonDown(0)){ //czy jest kliklięty lewy przycisk
                    if(dane.karta != null) { //jeśli jest wybrana karta cofająca
                        if(dane.numerKarty < 0){
                            dane.cofnieciaGracza += dane.losujCofniecie(dane.numerKarty);
                            dane.energiaGracza += dane.numerKarty;
                            dane.karta = null;
                            dane.correctUnoReverse();
                            AI();
                        }
                        else if(!alive){ //i pole jest puste
                            zaladowanaKarta = dane.numerKarty;          //załaduj kartę do pola
                            hp = dane.daneKart[zaladowanaKarta - 1,0];  //załaduj staty
                            atak = dane.daneKart[zaladowanaKarta - 1,1];
                            pionek.transform.position = gameObject.transform.position + new Vector3(0,0,100); //wejdź na górę
                            alive = true;   //ożyj
                            dane.karta = null;  //odznacz kartę
                            dane.numerKarty = 0; //odznacz kartę
                            dane.AddMove(dane.action.LoadCard, gameObject.name, zaladowanaKarta - 1, true);
                            dane.energiaGracza -= zaladowanaKarta - 1;
                            updateText();
                            AI();
                        } //jeśli jest wybrana karta i kliknięte pole jest załadowane to nie rób nic :3
                    }
                    else if(dane.pole == gameObject) dane.pole = null; //jeśli pole jest wybrane to odznacz
                    else dane.pole = gameObject; //inaczej zaznacz
                }
                gameObject.GetComponent<Renderer>().material.color = Color.blue; // kolor jesli myszka nad
            }   
		}
    }
    public void AI(){

        dane.wrogRusza = false;
        
        //wybierz czynność
        int czynnosc = Random.Range(1,4);  //1-zagraj karte, 2-walcz, 3-cofaj
        GameObject child, pole, poleGracza;
        int koszt, los;
        switch(czynnosc){
            case 1:
                los = Random.Range(1,7); //losuj numer karty do wybrania
                child = karty.transform.GetChild(los).gameObject; //uchwyt do karty
                koszt = child.GetComponent<karta>().rodzajKarty - 1; //koszt karty w energii
                if(koszt <= dane.energiaWroga){ //sprawdzamy czy możemy zagrac kartę, jesli nie to pasujemy
                    for(int i = 0; i < polaWroga.transform.childCount; i++){    //szukamy wolnego miejsca, jeśli takiego nie ma to pas
                        pole = polaWroga.transform.GetChild(i).gameObject; //uchwyt do znalezionego pola
                        if (!pole.transform.GetComponent<enemy>().alive){   //czy pole jest martwe (można postawić pionka)
                            pole.transform.GetComponent<enemy>().zaladowanaKarta = los; //ładujemy informacje karty do pola
                            pole.transform.GetComponent<enemy>().hp = dane.daneKart[los - 1,0];
                            pole.transform.GetComponent<enemy>().atak = dane.daneKart[los - 1,1];
                            pole.transform.GetComponent<enemy>().updateNeeded = true; //ustawiamy flagę update
                            dane.AddMove(dane.action.LoadCard, pole.name, koszt, false);    //rejestrujemy ruch
                            dane.energiaWroga -= koszt; //odejmujemy energię
                            print("ZALADOWANO KARTE");
                            break;
                        }
                    }
                }
                break;
            case 2:
                for(int i = 0; i < polaWroga.transform.childCount; i++){ //znajdz zywe pole wroga
                        pole = polaWroga.transform.GetChild(i).gameObject; //uchwyt do pola wroga
                        if (pole.transform.GetComponent<enemy>().alive){    //sprawdzamy czy jest na nim pionek (jest żywe)
                            for(int f = 0; f < polaGracza.transform.childCount; f++){   //szukamy teraz pionka gracza
                                poleGracza = polaGracza.transform.GetChild(f).gameObject;   //uchwyt do pionka
                                if (poleGracza.transform.GetComponent<Skrypt>().alive){//atakuj gracza jeśli pionek jest żywy
                                    dane.pole = null;   //odzaznacz pole (tak na wszelki wypadek)

                                    //sprawdzamy czy pionek wroga ma mniej lub tyle samo życia co atak pionka gracza, jeśli tak to rejestrujemy Śrierć, inaczej LoseHP
                                    if(poleGracza.transform.GetComponent<Skrypt>().atak >= pole.transform.GetComponent<enemy>().hp) dane.AddMove(dane.action.Die, pole.name, pole.transform.GetComponent<enemy>().hp, false);
                                    else dane.AddMove(dane.action.LoseHP, pole.name, poleGracza.transform.GetComponent<Skrypt>().atak, false);
                                    pole.transform.GetComponent<enemy>().hp -= poleGracza.transform.GetComponent<Skrypt>().atak;    //wróg traci hp
                                    pole.transform.GetComponent<enemy>().updateNeeded = true;   //ustawiamy flagę update dla wroga

                                    //to samo dla pionka gracza
                                    if(dane.silaAtakuWroga >= poleGracza.transform.GetComponent<Skrypt>().hp) dane.AddMove(dane.action.Die, poleGracza.name, poleGracza.transform.GetComponent<Skrypt>().hp, true);
                                    else dane.AddMove(dane.action.LoseHP, poleGracza.name, pole.transform.GetComponent<enemy>().atak , true);
                                    poleGracza.transform.GetComponent<Skrypt>().hp -= pole.transform.GetComponent<enemy>().atak; 
                                    poleGracza.transform.GetComponent<Skrypt>().updateNeeded = true;
                                    print("ATAK!"); //taka informacja bo czemu nie
                                    break;
                                }
                            }
                            break;
                        }
                    }
                break;
            case 3:
                los = -Random.Range(1,4); //losujemy kartę do cofania
                if(dane.energiaWroga < -los) { //sprawdzamy czy wroga stać na tą kartę
                    print("FAILED UNO REVERSE XD");
                    break;
                }
                dane.energiaWroga += los; //bóg zapłać
                if(Random.Range(0,2) == 0) dane.cofnieciaGracza += dane.losujCofniecie(los);//losujemy cofanie dla wroga albo gracza
                else dane.cofnieciaWroga += dane.losujCofniecie(los);//cofanie
                dane.karta = null;// usuwamy wybór karty bo czemu nie
                dane.correctUnoReverse(); //usuwamy nadmiarowe cofanie
                print("udane cofniecie"); //info
            break;
        }
        dane.energiaGracza += (dane.energiaGracza <= 4)? 1:0; //dodajemy ewnergię na koniec tury
        dane.energiaWroga += (dane.energiaWroga <= 4)? 1:0;
    }
}
