using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoCapController : MonoBehaviour {
    protected FileInfo theSourceFile;
    protected StreamReader reader = null;
    
    List<Quaternion> quaternionList = new List<Quaternion>();
    List<long> times = new List<long>();
    //Hack this shit together
    List<Quaternion> secondquaternionList = new List<Quaternion>();
    List<long> secondtimes = new List<long>();



    int counter = 0;
    bool done = false;
    Transform secondPhone;
    void Start()
    {
        theSourceFile = new FileInfo("Assets/recordedMoCapData0.txt");
        reader = theSourceFile.OpenText();

        readFiles(reader, ref quaternionList, ref times);
        theSourceFile = new FileInfo("Assets/recordedMoCapData1.txt");
        reader = theSourceFile.OpenText();
        readFiles(reader, ref secondquaternionList, ref secondtimes);

        secondPhone = this.transform.FindChild("SecondPivotPoint").FindChild("SecondPhone");

    }

    void Update()
    {
        
        if(counter < quaternionList.Count)
        {
            this.transform.rotation = quaternionList[counter];
            secondPhone.transform.rotation = secondquaternionList[counter];
            //Vector3 loc = (quaternionList[counter] * Vector3.forward);
            //loc = mirror.position + loc;
            //mirror.rotation = Quaternion.LookRotation(loc - mirror.position);
            //mirror.rotation = Quaternion.LookRotation(quaternionList[counter].eulerAngles);
            counter++;
        }
    }

    void readFiles(StreamReader reader, ref List<Quaternion> quaternionList, ref List<long> times)
    {
        string text = " ";
        quaternionList = new List<Quaternion>();
        string[] stringList;
        while (!done)
        {
            if ((text = reader.ReadLine()) != null)
            {
                //Console.WriteLine(text);
                stringList = text.Split(',', ' ');
                quaternionList.Add(new Quaternion(float.Parse(stringList[0]), float.Parse(stringList[1]), float.Parse(stringList[2]), float.Parse(stringList[3])));
                times.Add(Int64.Parse(stringList[5]));
            }
            else
            {
                done = true;
                Debug.Log(quaternionList.Count);
            }
        }
        done = false;
    }

    public void convertToCSV()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/moCapData");
        FileInfo[] info = dir.GetFiles("recordedMoCapData*.txt");
        StreamReader read = null;
        bool stillReading = true;
        string text = " ";
        string[] stringList;
        string[] tempStrings = new string[4];
        int count = 0;
        List<List<string[]>> dataList = new List<List<string[]>>();
        foreach (FileInfo f in info)
        {
            read = f.OpenText();
            dataList.Add(new List<string[]>());
            while (stillReading)
            {
                if ((text = read.ReadLine()) != null)
                {
                    stringList = text.Split(',', ' ');
                    dataList[count].Add(stringList);
                }
                else
                {
                    stillReading = false;
                }
            }
            count++;
            stillReading = true;
        }
        
        for (int i = 0; i < dataList.Count; i++)
        {
            string[] tempStringList = new string[dataList[i].Count];
            for (int j = 0; j < dataList[i].Count; j++)
            {
                //First CSV
                //x,y,z,w
                //x,y,z,w
                //x,y,z,w
                tempStringList[0] = tempStringList[0] + dataList[i][j][0] + "," + dataList[i][j][1] + "," + dataList[i][j][2] + "," + dataList[i][j][3] + "\n";
                //Second CSV 
                //x,x,x,x,x,x
                //y,y,y,y,y,y
                //z,z,z,z,z,z
                //w,w,w,w,w,w
                /*
                tempStringList[0] = tempStringList[0] + "," + dataList[i][j][0];
                tempStringList[1] = tempStringList[1] + "," + dataList[i][j][1];
                tempStringList[2] = tempStringList[2] + "," + dataList[i][j][2];
                tempStringList[3] = tempStringList[3] + "," + dataList[i][j][3];
                */
            }
            if(File.Exists("Assets/moCapData" + i + ".txt"))
            {
                File.Delete("Assets/moCapData" + i + ".txt");
            }
            //WHY WAS I STUCK ON THIS FOR AN HOUR?! Because the stupid Remove method does not manipulate the string in itself so it needs to be assigned back onto itself. I WANT POINTERS!
            //Second CSV
            /*
            tempStringList[0] = tempStringList[0].Remove(0, 1);
            tempStringList[1] = tempStringList[1].Remove(0, 1);
            tempStringList[2] = tempStringList[2].Remove(0, 1);
            tempStringList[3] = tempStringList[3].Remove(0, 1);*/
            File.WriteAllLines("Assets/moCapData" + i + ".txt", tempStringList);
        }
    }
}
