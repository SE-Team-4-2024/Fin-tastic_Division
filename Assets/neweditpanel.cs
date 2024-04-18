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
    void Start()
    {
     //disablebutton2();
    // disablebutton3();
     //disablebutton4();
    }
    public Button Button2;
      public Button Button1;
      public Button Button3;
      public Button Button4;
      public GameObject Object;
       public void disablebutton2()
      {
        //Button2.gameObject.SetActive(false);
         
         
      }
      public void enablebutton2()
      {
        // Button2.gameObject.SetActive(true);
      }
       public void disablebutton3()
      {
        //Button3.gameObject.SetActive(false);
         
         
      }
      public void enablebutton3()
      {
        // Button3.gameObject.SetActive(true);
      }
 public void disablebutton4()
      {
        //Button4.gameObject.SetActive(false);
         
         
      }
      public void enablebutton4()
      {
        // Button4.gameObject.SetActive(true);
      }


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
     public GameObject Newuserpanel;
         public void  openNewuserpanel() 
    {
        if( Newuserpanel!=null)
           Newuserpanel.SetActive(true);
    }
    public void exitNewuserpanel()
    {
          Newuserpanel.SetActive(false);
    }  
public void Switch()
{

   if(un1.text =="")
      {
        un1.text=username.text;
        username.text="";
         
      }
    else
    {
       if(un2.text =="")
       {
       un2.text=username.text;
       username.text="";
         enablebutton2();
       }
        else
      {
        if(un3.text =="")
       { 
          un3.text=username.text;
          username.text="";
           enablebutton3();

        }
        else
       {
        un4.text=username.text;
        username.text=""; 
        enablebutton4();
       }
      }
}
}
               public Text hello;
               public Toggle Toggle3;
       public Toggle Toggle2;
       public Toggle Toggle1;
       public Toggle Toggle4;


     
             public void opentoggle1() 
    {
        if(Toggle1.isOn)
        {
          hello.text=un1.text;
         // Toggle2.interactable=false;
          //Toggle3.interactable=false;
          //Toggle4.interactable=false;

        }
          else{
               
               hello.text="";
               // Toggle2.interactable=true;
         // Toggle3.interactable=true;
         // Toggle4.interactable=true;
           }
          
    }
    
     
     public Transform panelparent;

    public GameObject create;
    

    public void createpanels()
    {
      
      {
        GameObject newpanel=Instantiate(create,panelparent);
      }
    } 


}