using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System;
using System.IO;

public class RegressionScript : MonoBehaviour
{
    public TextAsset jsonText;
    public List<int> X = new List<int>();
    public List<int> Y = new List<int>();
    public int N;
    public double prediction;
    public GameObject detailObject;
    public TMP_InputField inputField;
    public GameObject predictionText;
    private bool showData;
    private string model;


    [Serializable]
    public class DataSet {
        public Rainfall[] Rainfall;
    }
    
    [Serializable]
    public class Rainfall
    {
        public int x;
        public int y;
    }

    public void LoadData()
    {
        DataSet data = JsonUtility.FromJson<DataSet>(jsonText.text);
        this.N = data.Rainfall.Length;
        
        for(int i=0;i<data.Rainfall.Length;i++)
        {
            this.X.Add(data.Rainfall[i].x);
            this.Y.Add(data.Rainfall[i].y);
        }

        Debug.Log("Sigma X : " + this.sigma(this.X));
        Debug.Log("Sigma X^2 : " + this.sigma(this.X, true));
        Debug.Log("Sigma Y : " + this.sigma(this.Y));
        Debug.Log("Sigma Y^2 : " + this.sigma(this.Y, true));
        Debug.Log("Sigma XY : " + this.sigmaXY());
        Debug.Log("Konstanta (a) : " + this.Konstanta());
        Debug.Log("Koefisien (b) : " + this.Koefisien());
        Debug.Log("Korelasi : " + this.Korelasi());
        Debug.Log("Hubungan : " + this.getHubungan());
        Debug.Log("Kekuatan : " + this.getKekuatan());
        Debug.Log("Koefisien Determinasi : " + this.Koefisien_determinasi());
        Debug.Log("Kontribusi Variabel Lain : " + this.Kontribusi_var_lain());

        ShowText();
    }

    void ShowText()
    {
        detailObject.transform.Find("Konstanta").GetComponent<TMP_Text>().text = "Konstanta: " + Math.Round(this.Konstanta(), 2);
        detailObject.transform.Find("Koefisien").GetComponent<TMP_Text>().text = "Koefisien: " + Math.Round(this.Koefisien(), 2);
        detailObject.transform.Find("Korelasi").GetComponent<TMP_Text>().text = "Korelasi: " + this.Korelasi();
        detailObject.transform.Find("Hubungan").GetComponent<TMP_Text>().text = "Hubungan: " + this.getHubungan();
        detailObject.transform.Find("Kekuatan").GetComponent<TMP_Text>().text = "Kekuatan: " + this.getKekuatan();
        detailObject.transform.Find("Determinasi").GetComponent<TMP_Text>().text = "Koefisien Determinasi : " + this.Koefisien_determinasi();
        detailObject.transform.Find("Kontribusi").GetComponent<TMP_Text>().text = "Kontribusi Variabel Lain : " + this.Kontribusi_var_lain();
        
        string sign = "+";
        if (this.Koefisien() < 0)
        {
            sign = "-";
        }
        else
        {
            sign = "+";
        }
        this.model = "Model : " + "Y = " + Math.Round(this.Konstanta(), 2) + sign + Math.Abs(Math.Round(this.Koefisien(), 2)) + " X";

        detailObject.transform.Find("Model").GetComponent<TMP_Text>().text = this.model;
        this.showData = true;
    }

    public void Prediksi()
    {
        if(showData == true)
        {
            if (inputField.text != "")
            {
                double _inputField = double.Parse(inputField.text);
                this.prediction = this.Konstanta() + (this.Koefisien() * _inputField);
                predictionText.SetActive(true);
                predictionText.GetComponent<TMP_Text>().text = "Suhu (c) : " + this.prediction;
                detailObject.transform.Find("Kesimpulan").gameObject.SetActive(true);
                detailObject.transform.Find("Kesimpulan").GetComponent<TMP_Text>().text = 
                    "Besar hubungan antara intensitas hujan dan suhu adalah " + this.Korelasi() + "\n" +
                    "Di mana jenis hubungannya yaitu " + this.getHubungan() + "\n" +
                    "dan kekuatannya yaitu " + this.getKekuatan() + "\n\n" +
                    "besar kontribusi dari intensitas hujan terhadap suhu adalah " + this.Koefisien_determinasi() + "% \n" +
                    "sedangkan "+ this.Kontribusi_var_lain() +"% merupakan kontribusi dari variabel lain";
            }
            else
            {
                Debug.Log("Check your input");
                predictionText.SetActive(false);
            }
        }
        else
        {
            Debug.Log("You should show data first!");
        }
        
    }

    public void EksporCSV()
    {
        string path = EditorUtility.OpenFolderPanel("Export file to folder", "", "");
        if (path.Length != 0)
        {
            string filename = path + "/export_cuaca" + ".csv";

            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Konstanta,Koefisien,Korelasi,Hubungan,Kekuatan,KoefisienDeterminasi,KontribusiVarLain,Model,NilaiInput,Hasil");
            
            tw.Close();

            tw = new StreamWriter(filename, true);
            tw.WriteLine(Math.Round(this.Konstanta(), 2) + "," + Math.Round(this.Koefisien(), 2) + "," + this.Korelasi() + "," + this.getHubungan() + "," + this.getKekuatan() + "," 
                + this.Koefisien_determinasi() + "," + this.Kontribusi_var_lain() + "," + this.model + "," + double.Parse(inputField.text) + "," +Math.Round(this.prediction, 2));
            tw.WriteLine();
            tw.Close();
            Debug.Log("Data exported successfully");
        }
        else
        {
            Debug.Log("Could not find the path location");
        }
    }

    private double sigma(List<int> data, bool power = false)
    {
        double result = 0;
        if(power == false)
        {
            for(int i=0; i < data.ToArray().Length; i++)
            {
                result += data.ToArray()[i];
            }
            return result;
        }
        else
        {
            for (int i = 0; i < data.ToArray().Length; i++)
            {
                result += Math.Pow(data.ToArray()[i], 2);
            }
            return result;
        }
    }

    private double sigmaXY()
    {
        double result = 0;
        for (int i = 0; i < this.X.ToArray().Length; i++)
        {
            result += this.X.ToArray()[i] * this.Y.ToArray()[i];
        }
        return result;
    }

    public double Konstanta()
    {
        double atas = (this.sigma(this.Y) * this.sigma(this.X, true)) - (this.sigma(this.X) * this.sigmaXY());
        double bawah = (this.N * this.sigma(this.X, true)) - Math.Pow(this.sigma(this.X), 2);
        double hasilKonstanta = atas / bawah;
        return hasilKonstanta;

    }

    public double Koefisien()
    {
        double atas = (this.N * this.sigmaXY()) - (this.sigma(this.X) * this.sigma(this.Y));
        double bawah = (this.N * this.sigma(this.X, true)) - Math.Pow(this.sigma(this.X), 2);
        double hasilKonstanta = atas / bawah;
        return hasilKonstanta;
    }

    public double Korelasi()
    {
        double atas = (this.N * this.sigmaXY()) - (this.sigma(this.X) * this.sigma(this.Y));
        double bawah_kiri = (this.N * this.sigma(this.X, true)) - Math.Pow(this.sigma(this.X), 2);
        double bawah_kanan = (this.N * this.sigma(this.Y, true)) - Math.Pow(this.sigma(this.Y), 2);
        double hasil_bawah = Math.Sqrt(bawah_kiri * bawah_kanan);
        double hasilKorelasi = atas / hasil_bawah;
        return hasilKorelasi;
    }

    public string getHubungan()
    {
        if ( Korelasi() < 0)
        {
            return "negatif";
        }
        else
        {
            return "positif";
        }
    }

    public string getKekuatan()
    {
        var kekuatan = Korelasi();
        if (Math.Abs(kekuatan) < 0.2)
        {
            return "sangat lemah";
        }
        else if (Math.Abs(kekuatan) >= 0.2 && Math.Abs(kekuatan) < 0.4)
        {
            return "lemah";
        }
        else if (Math.Abs(kekuatan) >= 0.4 && Math.Abs(kekuatan) < 0.6)
        {
            return "sedang";
        }
        else if (Math.Abs(kekuatan) >= 0.6 && Math.Abs(kekuatan) < 0.8)
        {
            return "kuat";
        }
        else if (Math.Abs(kekuatan) >= 0.8 && Math.Abs(kekuatan) <= 1)
        {
            return "lemah";
        }
        else
        {
            return "tidak terdefinisi";
        }
    }

    public double Koefisien_determinasi()
    {
        double koef_deter = Math.Round(Math.Pow(this.Korelasi(), 2) * 100, 1);
        return koef_deter;
    }

    public double Kontribusi_var_lain()
    {
        double kontribusi = Math.Round(100 - this.Koefisien_determinasi(), 1);
        return kontribusi;
    }
}
