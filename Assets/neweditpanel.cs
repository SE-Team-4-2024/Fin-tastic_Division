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
        
          Userpanel1.SetActive(true);
    }
    public void exitopenusername1()
    {
         Userpanel1.SetActive(false);
    }
    public InputField username;
      
  
      public Text un1;


     
     
      public void save1()
      {
         un1.text=username.text;
        username.text="";
       
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
    //else
    //{
      // if(un2.text =="")
       //{
       //un2.text=username.text;
       //username.text="";
       //  enablebutton2();
      // }
       // else
      //{
        //if(//un3.text =="")
      // { 
          //un3.text=username.text;
         // username.text="";
         //  enablebutton3();

       // }
       
     // }
//}
}
               public Text hello;
               
       
       public Toggle Toggle1;
      


     
             public void opentoggle1() 
    {
        if(Toggle1.isOn)
        {
          hello.text=un1.text;
         

        }
          else{
               
               hello.text="";
               
           }
          
    }
    
     
     public Transform panelparent;
     

    public GameObject create;
    
    

    public void createpanels()
    {
      
      
         GameObject newpanel=Instantiate(create,panelparent);
      newpanel.GetComponentInChildren<Text>().text =Newuserpanel.GetComponentInChildren<InputField>().text;
      // newpanel.GetComponentInChildren<Text>().text = Userpanel1.GetComponentInChildren<InputField>().text;
       Newuserpanel.GetComponentInChildren<InputField>().text="";
    }
     public void ShowDetails(GameObject newpanel)
     {
        newpanel.GetComponentInChildren<Text>().text = Newuserpanel.GetComponentInChildren<InputField>().text;
       //Newuserpanel.GetComponentInChildren<InputField>().text="";
      }
     public void ShowDetails1(GameObject newpanel)
     {
       newpanel.GetComponentInChildren<Text>().text = Userpanel1.GetComponentInChildren<InputField>().text;
       Userpanel1.GetComponentInChildren<InputField>().text="";
     }
     //  if(newpanel.GetComponentInChildren<Text>().text=="") {
     //   newpanel.GetComponentInChildren<Text>().text = Newuserpanel.GetComponentInChildren<InputField>().text;
     //   Newuserpanel.GetComponentInChildren<InputField>().text="";
     //  }

           
       //    else{
       // newpanel.GetComponentInChildren<Text>().text = Userpanel1.GetComponentInChildren<InputField>().text;
       // Userpanel1.GetComponentInChildren<InputField>().text="";

    //  }
   // } 
   // }
        //public Text someTextComponent;   
        // public InputField someInputField;
       
      // public void press(){
      //   GameObject textObject = GameObject.Find("user1");
       //  GameObject inputFieldObject = GameObject.Find("userpanel1 (1)");
      // Text textComponent = textObject.GetComponentInChildren<Text>();
      // InputField inputField = inputFieldObject.GetComponentInChildren<InputField>();
        
      //  textComponent.text = inputField.text;
        }
//public GameObject textprefab;
 //  public void textcreate()
   //   {
      //   GameObject newtext=Instantiate(textprefab,transform);
      //  newtext.GetComponentInChildren<Text>().text =Userpanel1.GetComponentInChildren<InputField>().text;
      // }
//}