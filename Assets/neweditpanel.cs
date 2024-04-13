using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class neweditpanel : MonoBehaviour
{
    // Start is called before the first frame update

public GameObject Edituserpanel;
    public void openedit() 
    {
        if(Edituserpanel!=null)
          Edituserpanel.SetActive(true);
    }
    public void exitopenedit()
    {
         Edituserpanel.SetActive(false);
    }
    public GameObject Userpanel1;
    public void openusername1() 
    {
        if(Userpanel1!=null)
          Userpanel1.SetActive(true);
    }
    public void exitopenusername1()
    {
         Userpanel1.SetActive(false);
    }
    public InputField username;
      
  
      public Text un1;
      public Text un2;
      public Text un3;
      
      public Text un4;
     
      public void save1()
      {
         un1.text=username.text;
        username.text="";
       
           }
           public void save2()
      {
         un2.text=username.text;
        username.text="";
       
           }
      
       public void save3(){

           un3.text=username.text;
           username.text="";
       }
        public void save4(){

           un4.text=username.text;
           username.text="";
       }
       public GameObject Userpanel2;
         public void openusername2() 
    {
        if(Userpanel2!=null)
          Userpanel2.SetActive(true);
    }
    public void exitopenusername2()
    {
         Userpanel2.SetActive(false);
    }  
    public GameObject Userpanel3;
         public void openusername3() 
    {
        if(Userpanel3!=null)
          Userpanel3.SetActive(true);
    }
    public void exitopenusername3()
    {
         Userpanel3.SetActive(false);
    }  
    public GameObject Userpanel4;
         public void openusername4() 
    {
        if(Userpanel4!=null)
          Userpanel4.SetActive(true);
    }
    public void exitopenusername4()
    {
         Userpanel4.SetActive(false);
    }  
    public GameObject Settingspanel;
         public void opensettings() 
    {
        if(Settingspanel!=null)
          Settingspanel.SetActive(true);
    }
    public void exitsettings()
    {
         Settingspanel.SetActive(false);
    }  
        
      
      
}