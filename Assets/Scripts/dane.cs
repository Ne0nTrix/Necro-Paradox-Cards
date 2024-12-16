using UnityEngine;

public static class dane
{
    public enum action{ //RODZAJ AKCJI
        LoseHP,
        LoadCard,
        Die,
        Null
    }

    // public static (action Akcja, GameObject Pole, int Wartosc)[3] ruchy;
    // public static (action Akcja, GameObject Pole, int Wartosc)[3] enemyRuchy;

    public static action[] meAkcje = {action.Null,action.Null,action.Null}; //lista akcjic(lista cofanie)
    public static string[] mePole = {null,null,null};
    public static int[] meWartosc = {0,0,0};


    public static action[] enemyAkcje = {action.Null,action.Null,action.Null}; //lista cofanie
    public static string[] enemyPole = {null,null,null};
    public static int[] enemyWartosc = {0,0,0};



    public static GameObject karta = null, pole = null, enemySelectedPole = null, enemyKarta = null; //wybrana karta (kontrola gry)
    public static int numerKarty = 0 , enemyKartNum = 0; //numer wybranej karty
    public static bool ruchAtak = false; //czy ma zostac policzony atak
    public static int silaAtakuWroga = 0; //jak mocno atakuje wrog

    public static int cofnieciaGracza = 0, cofnieciaWroga = 0; //ile do cofniecia po kazdej stronie

    public static int energiaGracza = 0, energiaWroga = 0; //energia uczestnikow

    public static bool wrogRusza = false; //czy wrog ma ruszyc

    public static int[,] daneKart = {{3,1,0},{5,2,1},{10,5,2},{16,8,3},{7,15,4},{30,20,5}}; //dane kart w kolejnosci
    // you can add more variables here

    public static void AddMove(action typAkcji, string poleHandle, int wartosc, bool myMove){ //dodanie ruchu
        if(myMove){ //jesli dodajemy ruch gracza
            for(int i = 2; i > 0; i--){//przesun tablice do przodu
                meAkcje[i] = meAkcje[i-1];
                mePole[i] = mePole[i-1];
                meWartosc[i] = meWartosc[i-1];
            }

                meAkcje[0] = typAkcji;//dodaj na poczatek
                mePole[0] = poleHandle;
                meWartosc[0] = wartosc;
        }
        else{ //jesli dodajemy ruch wroga
            for(int i = 2; i > 0; i--){ //przesun tablice do przodu
                enemyAkcje[i] = enemyAkcje[i-1];
                enemyPole[i] = enemyPole[i-1];
                enemyWartosc[i] = enemyWartosc[i-1];
            }

                enemyAkcje[0] = typAkcji; //dodaj na poczatek
                enemyPole[0] = poleHandle;
                enemyWartosc[0] = wartosc;
        }
    }

    public static void UnoReverse(bool myMove){ //cofamy ruch
        if(myMove){ //cofamy gracza
            for(int i = 0; i <=1; i++){//cofamy tablice
                meAkcje[i] = meAkcje[i+1];
                mePole[i] = mePole[i+1];
                meWartosc[i] = meWartosc[i+1];
            }

            meAkcje[2] = action.Null; //na koncu dajemy null
            mePole[2] = null;
            meWartosc[2] = 0;
            cofnieciaGracza--;
        }
        else{ //cofamy wroga
            for(int i = 0; i <= 1; i++){
                enemyAkcje[i] = enemyAkcje[i+1];//cofamy tablice
                enemyPole[i] = enemyPole[i+1];
                enemyWartosc[i] = enemyWartosc[i+1];
            }

                enemyAkcje[2] = action.Null;//na koncu dajemy null
                enemyPole[2] = null;
                enemyWartosc[2] = 0;
                cofnieciaWroga--;
        }
        correctUnoReverse();
    }

    public static void correctUnoReverse(){ //kasujemy nadmierne cofnięcia jeśli listy są puste
        if(enemyAkcje[0] == action.Null) cofnieciaWroga = 0;
        if (meAkcje[0] == action.Null) cofnieciaGracza = 0;
    }

    public static int losujCofniecie(int nrKarty){ //losowanie cofnieć
        int oczka = Random.Range(1,7) + Random.Range(1,7);
        switch (nrKarty){
            case -1:
                if(oczka <= 6) return 1;
                else return 2;
            case -2:
                if (oczka <= 4) return 1;
                else if (oczka <= 8) return 2;
                else return 3;
            case -3:
                if (oczka <= 6) return 2;
                else return 3;
            default:
                return 1;
        }
    }
}