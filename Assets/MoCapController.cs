using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoCapController : MonoBehaviour {
    protected FileInfo theSourceFile;
    protected StreamReader reader = null;
	
    Transform secondPhone;
	
    //Quaternion and time lists to store our motion data
    List<Quaternion> quaternionList = new List<Quaternion>();
    List<long> times = new List<long>();
	
    //Hack this shit together
    List<Quaternion> secondquaternionList = new List<Quaternion>();
    List<long> secondtimes = new List<long>();

	int counter = 0;
    bool done = false;

    
    void Start()
    {
        theSourceFile = new FileInfo("Assets/recordedMoCapData0.txt");
        reader = theSourceFile.OpenText();
        readFiles(reader, ref quaternionList, ref times);
		
        theSourceFile = new FileInfo("Assets/recordedMoCapData1.txt");
        reader = theSourceFile.OpenText();
        readFiles(reader, ref secondquaternionList, ref secondtimes);
		
		//We need a referenced to the second phone so we can edit its rotations
        secondPhone = this.transform.FindChild("SecondPivotPoint").FindChild("SecondPhone");

    }

    void Update()
    {
		//Start rotating the phones.
        if(counter < quaternionList.Count)
        {
            this.transform.rotation = quaternionList[counter];
            secondPhone.transform.rotation = secondquaternionList[counter];
            counter++;
        }
    }
	//I pretty much ripped the reading from some stackoverflow post and customized it. It is really ugly and should be fixed.
    void readFiles(StreamReader reader, ref List<Quaternion> quaternionList, ref List<long> times)
    {
        string text = " ";
        quaternionList = new List<Quaternion>();
        string[] stringList;
		//Not very proud of this while setup -.-
        while (!done)
        {
            if ((text = reader.ReadLine()) != null)
            {
				//text.Split is so useful! Love that method.
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

	//Converting to CSV is incredibly useful when working with stupid programs that require excel documents.
    public void convertToCSV()
    {
		//Find the applications datapath and append the folder moCapData to it.
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/moCapData");
		//Get the files from that dir.
        FileInfo[] info = dir.GetFiles("recordedMoCapData*.txt");
        StreamReader read = null;
        bool stillReading = true;
        string text = " ";
        string[] stringList;
        string[] tempStrings = new string[4];
        int count = 0;
        List<List<string[]>> dataList = new List<List<string[]>>();
		//Read the data from those files and append to a list of lists of a string array
		//I need it that convoluted, I promise. I thought I didn't at the start, but it turned out that way.
        foreach (FileInfo f in info)
        {
            read = f.OpenText();
            dataList.Add(new List<string[]>());
            while (stillReading)
            {
                if ((text = read.ReadLine()) != null)
                {
					//I still love string.Split().
                    stringList = text.Split(',', ' ');
                    dataList[count].Add(stringList);
                }
                else
                {
                    stillReading = false;
                }
            }
			//I could have probably done this in a for loop. But meh.
            count++;
			//I really don't like myself for using while loops and bools -.-
            stillReading = true;
        }
        //I don't know why I needed the second CSV format. I made that thing first for some reason. It is there if I decide I need it.
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
			//Could not be bothered figuring out how to overwrite the files, so I just delete them.
            if(File.Exists("Assets/moCapData" + i + ".txt"))
            {
                File.Delete("Assets/moCapData" + i + ".txt");
            }
            //WHY WAS I STUCK ON THIS FOR AN HOUR?! Because the stupid Remove method does not manipulate the string in itself so it needs to be assigned back onto itself. I WANT POINTERS!
			//Don't worry, you didn't even need it -.-
            //Second CSV
            /*
            tempStringList[0] = tempStringList[0].Remove(0, 1);
            tempStringList[1] = tempStringList[1].Remove(0, 1);
            tempStringList[2] = tempStringList[2].Remove(0, 1);
            tempStringList[3] = tempStringList[3].Remove(0, 1);*/
			
			//Just toss the data into the root folder. No clutter.
            File.WriteAllLines("Assets/moCapData" + i + ".txt", tempStringList);
        }
    }
}
