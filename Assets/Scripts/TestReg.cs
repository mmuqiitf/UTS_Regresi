using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestReg : MonoBehaviour
{
    public TextAsset jsonText;
    public int banyakN;
    public int sumX, sumX2;
    public int sumY, sumY2;
    public int sum_XY;
    public double Nilai_Konstanta;
    public double Nilai_Koefisien;
    public double Nilai_Korelasi;
    //public int atas, bawah;

    [Serializable]
    public class DataSet
    {
        public Values[] Values;
    }

    [Serializable]
    public class Values
    {
        public int x;
        public int y;
    }

    // void Start()
    // {
    //     DataSet data = JsonUtility.FromJson<DataSet>(jsonText.text);
    //     for(int i=0;i<data.Values.Length;i++)
    //     {
    //         Debug.Log("x: "+data.Values[i].x);
    //         Debug.Log("y: "+data.Values[i].y);
    //     }
    // }

    //private void Start()
    //{
    //    DataSet data = JsonUtility.FromJson<DataSet>(jsonText.text);
    //    int sum_x = 0;
    //    int sum_y = 0;
    //    int n = data.Values.Length;

    //    for (int i = 0; i < data.Values.Length; i++)
    //    {
    //        sum_x = sum_x + data.Values[i].x;
    //        sum_y = sum_y + data.Values[i].y;
    //    }

    //    Debug.Log("Banyak Data: " + n);
    //    Debug.Log("SUM X: " + sum_x);
    //    Debug.Log("SUM Y: " + sum_y);
    //}

    public void Regresi()
    {
        DataSet data = JsonUtility.FromJson<DataSet>(jsonText.text);
        int sum_x = 0;
        int sum_y = 0;
        int sum_x2 = 0;
        int sum_y2 = 0;
        int sum_xy = 0;
        int n = data.Values.Length;

        for (int i = 0; i < data.Values.Length; i++)
        {
            sum_x = sum_x + data.Values[i].x;
            sum_y = sum_y + data.Values[i].y;
            sum_x2 += data.Values[i].x * data.Values[i].x;
            sum_y2 += data.Values[i].y * data.Values[i].y;
            sum_xy += data.Values[i].x * data.Values[i].y;
        }
        this.banyakN = n;
        this.sumX = sum_x;
        this.sumY = sum_y;
        this.sumX2 = sum_x2;
        this.sumY2 = sum_y2;
        this.sum_XY = sum_xy;
        Debug.Log("Banyak Data: " + n);
        Debug.Log("SUM X: " + sum_x);
        Debug.Log("SUM Y: " + sum_y);
        Debug.Log("SUM X^2: " + sum_x2);
        Debug.Log("SUM Y^2: " + sum_y2);
        Debug.Log("SUM X*Y: " + sum_xy);

        double konstanta = this.Konstanta();
        this.Nilai_Konstanta = konstanta;
        Debug.Log("Konstanta: " + konstanta);

        double koefisien = this.Koefisien();
        this.Nilai_Koefisien = koefisien;
        Debug.Log("Koefisien: " + koefisien);

        double korelasi = this.Korelasi();
        this.Nilai_Korelasi = korelasi;
        Debug.Log("Korelasi : " + korelasi);

        string hubungan = this.Hubungan();
        Debug.Log("Hubungan: " + hubungan);

        string kekuatan = this.Kekuatan();
        Debug.Log("Kekuatan: " + kekuatan);

        double koefisien_determinasi = this.Koefisien_determinasi();
        Debug.Log("Koefisien Determinasi: " + koefisien_determinasi + "%");

        double kontrib_var_lain = this.Kontribusi_var_lain();
        Debug.Log("Kontribusi Variabel Lain: " + kontrib_var_lain);

    }

    public double Konstanta()
    {
        int atas = ((this.sumY * this.sumX2) - (this.sumX * this.sum_XY));
        double bawah = (this.banyakN) * (this.sumX2) - Math.Pow(this.sumX, 2);
        double hasilKonstanta = atas / bawah;
        return hasilKonstanta;

    }

    public double Koefisien()
    {
        int atas = (this.banyakN * this.sum_XY) - (this.sumX * this.sumY);
        double bawah = (this.banyakN * this.sumX2) - Math.Pow(this.sumX, 2);
        double hasilKoefisien = atas / bawah;
        return hasilKoefisien;

    }

    public double Korelasi()
    {
        int atas = (this.banyakN * this.sum_XY) - (this.sumX * this.sumY);
        double bawah_kiri = (this.banyakN * this.sumX2) - Math.Pow(this.sumX, 2);
        double bawah_kanan = (this.banyakN * this.sumY2) - Math.Pow(this.sumY, 2);
        double hasil_bawah = Math.Sqrt(bawah_kiri * bawah_kanan);
        double hasilKorelasi = atas / hasil_bawah;
        return hasilKorelasi;
    }

    public string Hubungan()
    {
        var korelasi = Korelasi();
        if (korelasi < 0)
        {
            return "negatif";
        }
        else
        {
            return "positif";
        }


    }

    public string Kekuatan()
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
