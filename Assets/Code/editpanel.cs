using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class editpanel : MonoBehaviour
{

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
    public GameObject Settingspanel;
    public void openusername() 
    {
        if(Settingspanel!=null)
          Settingspanel.SetActive(true);
    }
    public void exitopenusername()
    {
         Settingspanel.SetActive(false);
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
       public GameObject Settingspanel2;
         public void openusername2() 
    {
        if(Settingspanel2!=null)
          Settingspanel2.SetActive(true);
    }
    public void exitopenusername2()
    {
         Settingspanel2.SetActive(false);
    }  
    public GameObject Settingspanel3;
         public void openusername3() 
    {
        if(Settingspanel3!=null)
          Settingspanel3.SetActive(true);
    }
    public void exitopenusername3()
    {
         Settingspanel3.SetActive(false);
    }  
    public GameObject Settingspanel4;
         public void openusername4() 
    {
        if(Settingspanel4!=null)
          Settingspanel4.SetActive(true);
    }
    public void exitopenusername4()
    {
         Settingspanel4.SetActive(false);
    }  
    public GameObject Settingspanelreal;
         public void openusernamereal() 
    {
        if(Settingspanelreal!=null)
          Settingspanelreal.SetActive(true);
    }
    public void exitopenusernamereal()
    {
         Settingspanelreal.SetActive(false);
    }  
        
      
      
}
    
