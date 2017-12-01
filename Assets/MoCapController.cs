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
            counter++; counter++;
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
                for (int i = 0; i < stringList.Length; i++)
                {
                    quaternionList.Add(new Quaternion(float.Parse(stringList[0]), float.Parse(stringList[1]), float.Parse(stringList[2]), float.Parse(stringList[3])));
                    times.Add(Int64.Parse(stringList[5]));
                }
            }
            else
            {
                done = true;
                Debug.Log(quaternionList.Count);
            }
        }
        done = false;
    }


}
