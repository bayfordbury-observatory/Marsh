#include <main.h>

//Declare variables
int16 pwm, adcX, adcY;
signed int16 gX, gY;

int8 light_pwm;
int1 light_flag;

int8 menu=0;

int8 val;

int8 in, command;

int8 datain[4];

int8 numin[4];

int8 num_index;

   int8 speed=0;
   int1 slewing=0;
   int1 tracking=0;
  

int8 DEC_out, RA_out, lastDEC_out, lastRA_out;

int8 Focus_out,lastFocus_out;

int8 solarSystem=0;
int8 messier=0;

int8 sendTarget=0;
int1 sentTarget=0;
//0 =none
//1 =solar system

char solarSystemStrings[9][20] = {"Mercury            ", "Venus              ", "Mars               ", "Jupiter            ", "Saturn             ", "Uranus             ", "Neptune            ", "Moon               ", "Sun                "};


//Initialise LCD
char text1[20]="Marsh Telescope    ";
char text2[20]="Control System V2.1";

const char Slave = 0x78;
const char Comsend = 0x00;
const char Datasend = 0x40;

const char Line1 = 0x80;
const char Line2 = 0xC0;

//Clear LCD
void blank(void){
   text1="                   ";
   text2="                   ";
}

//Next line of LCD
void nextline(void){
   i2c_start ();
   i2c_write(Slave);
   i2c_write(Comsend);
   i2c_write(Line2);
   i2c_stop ();
}

//First line of LCD
void firstline(void){
   i2c_start ();
   i2c_write(Slave);
   i2c_write(Comsend);
   i2c_write(Line1);
   i2c_stop ();
}

//Write new text to LCD
void Show(unsigned char *text){
   int n;//,d;
   //d=0x00;
   i2c_start ();;
   i2c_write(Slave);
   i2c_write(Datasend);
   i2c_write(' ');
   for(n=0;n<19;n++){
      i2c_write(*text);
      ++text;
   }
   i2c_stop ();
}

//Write both lines to LCD
void WriteLCD(void){
   firstline();
   Show(text1);
   nextline();
   Show(text2);
}

void clearNumin(int8 index){

   numin[0]=0;
   numin[1]=0;
   numin[2]=0;
   numin[3]=0;
   
   num_index=index;
   
}

void enterNum(int8 num){

   numin[num_index]=num;
   switch(menu){
   case M_MESSIER:
      if(num_index==0){
         num_index=2;
      }else{
         num_index--;
      }
      break;
   }
}

//Change on PORT B pin
//Potential use for button press ISR, but not currently used
#int_RB
void  RB_isr(void) {

}

#int_RDA
//Data received from controller
void  RDA_isr(void) {
   in = fgetc(controller);

   if(in=='#'){
      in = fgetc(controller);
      
      switch(in){
      case 'R':         
         //read eeprom
         
         datain[0]=fgetc( controller);//address msb
         datain[1]=fgetc( controller);//address lsb
         
         fputc('$', controller);
         fputc('R', controller);
         fputc(read_eeprom((datain[0]<<8) | (datain[1])), controller);
      
         break;
      case 'r':
         //Write eeprom         
         datain[0]=fgetc( controller);//address msb
         datain[1]=fgetc( controller);//address lsb
         datain[2]=fgetc( controller);//data

         write_eeprom((datain[0]<<8) | (datain[1]), datain[1]);
         break;
         
      case 't':
         //set tracking
         
         command=fgetc(controller);
         
         if(command==0x40){
            tracking=0;            
         }else if(command==0x41){
            tracking=1;
         }
         break;
      }
   }
}


void main(){


   setup_ccp2(CCP_PWM);
   setup_timer_2(T2_DIV_BY_16, 255, 1);
   set_pwm2_duty(0);
   setup_timer_3(T3_DISABLED|T3_DIV_BY_1);
   setup_timer_4(T4_DISABLED,0,1);
   set_pwm2_duty(0);  
   
   i2c_start ();
   if (i2c_write(slave)==0){
      i2c_write(Comsend);
      i2c_write(0x38); //function set
      delay_ms(10);
      i2c_write(0x39); //function set
      delay_ms(10);
      i2c_write(0x14); //int osc freq
      i2c_write(0x79); //contrast set
      i2c_write(0x50); //power/icon/contrast set
      i2c_write(0x6C); //follower control
      i2c_write(0x0C); //display off
      i2c_write(0x01); //clear display
      i2c_write(0x06);
      delay_ms(10);
   }
   i2c_stop ();

   Show(text1);
   nextline();
   Show(text2);

   port_b_pullups(FALSE);
   output_float(KEYPAD1);
   output_float(KEYPAD2);
   output_float(KEYPAD3);
   output_float(KEYPAD4);
   output_float(KEYPAD5);
   output_float(KEYPAD6);
   output_float(KEYPAD7);
   output_float(KEYPAD8);
   output_float(KEYPAD9);

   setup_comparator(NC_NC_NC_NC);

   setup_vref(VREF_4v096);
   setup_adc_ports(sAN0|sAN1|VSS_FVR);
   setup_adc(ADC_CLOCK_DIV_64|ADC_TAD_MUL_16);


   for(pwm=0; pwm<1024; pwm++){
      set_pwm2_duty(pwm);
      delay_ms(1);
   }
   for(pwm=1023; pwm>200; pwm--){
      set_pwm2_duty(pwm);
      delay_ms(1);
   }
   delay_ms(200); 

   //Enable interrupt when data received from controller
   enable_interrupts(INT_RDA);
   //enable_interrupts(INT_RB);
   enable_interrupts(GLOBAL);


   //Main loop
   while(true){

      //Test for button presses


      //No slew unless notified otherwise
      slewing=0;

      //Default bitmask (stop slewing)
      RA_out =0b11100000;
      DEC_out=0b10100000;
      Focus_out=0;


      //black
      output_low(KEYPAD2);
      if(!input(KEYPAD6)){
         delay_ms(20);
         if(!input(KEYPAD6)){// up /north
            slewing=1;
            DEC_out=0b10101000 + speed;
         }
      }else if(!input(KEYPAD7)){
         delay_ms(20); 
         if(!input(KEYPAD7)){// down /south
            slewing=1;
            DEC_out=0b10001000 + speed;
         }
      }
      if(!input(KEYPAD8)){
         delay_ms(20); 
         if(!input(KEYPAD8)){// right /west
            slewing=1;
            RA_out=0b11101000 + speed;
         }
      }else if(!input(KEYPAD9)){
         delay_ms(20); 
         if(!input(KEYPAD9)){// left /east
            slewing=1;
            RA_out=0b11001000 + speed;
         }
      }
      output_float(KEYPAD2);

      if(!slewing){
         //green
         output_low(KEYPAD1);
         if(!input(KEYPAD6)){
            delay_ms(20);
            if(!input(KEYPAD6)){// (0) LIGHT
               val=0;
               switch(menu){
               case M_BASE: menu=M_LIGHT; break;
               case M_LIGHT: light_flag=1;light_pwm=0; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }      
               while(!input(KEYPAD6)){
                  delay_ms(10);
               }
               delay_ms(20);
            }
         }else if(!input(KEYPAD7)){ 
            delay_ms(20); 
            if(!input(KEYPAD7)){// (1) SPEED
               val=1;
               switch(menu){
               case M_BASE: menu=M_SPEED; break;
               case M_SPEED: speed=0; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=5; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD7)){
                  delay_ms(10);
               }
               delay_ms(50);
            }
         }else if(!input(KEYPAD8)){ 
            delay_ms(20); 
            if(!input(KEYPAD8)){// (2) CALDWELL
               val=2;
               switch(menu){
               case M_SPEED: speed=1; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=15; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD8)){
                  delay_ms(10);
               }
               delay_ms(50);
            }
         }else if(!input(KEYPAD9)){ 
            delay_ms(20); 
            if(!input(KEYPAD9)){// (3) MESSIER
               val=3;
               switch(menu){
               //case M_BASE: clearNumin(2); menu=M_MESSIER; break;
               case M_SPEED: speed=2; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=40; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD9)){
                  delay_ms(10);
               }
               delay_ms(50);     
            }
         }
         output_float(KEYPAD1);
      }


      if(!slewing){
         //red
         output_low(KEYPAD5);
         if(!input(KEYPAD6)){ // (4) FOCUS
            delay_ms(20);
            if(!input(KEYPAD6)){
               val=4;
               switch(menu){
               case M_BASE: menu=M_FOCUS; break;
               case M_SPEED: speed=3; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=60; menu=M_BASE; break;
               case M_FOCUS: menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD6)){
                  delay_ms(10);
               }
               delay_ms(50);
            }
         }else if(!input(KEYPAD7)){ // (5) SOLAR SYSTEM
            delay_ms(20); 
            if(!input(KEYPAD7)){
               val=5;
               switch(menu){
               case M_BASE: menu=M_SOLARSYSTEM; break;
               case M_SPEED: speed=4; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=100; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD7)){
                  delay_ms(10);
               }
               delay_ms(50);
            }
         }else if(!input(KEYPAD8)){ // (6) STAR
            delay_ms(20); 
            if(!input(KEYPAD8)){
               val=6;
               switch(menu){
               case M_SPEED: speed=5; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=150; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD8)){
                  delay_ms(10);
               }
               delay_ms(50);       
            }
         }else if(!input(KEYPAD9)){ // (7) RET
            delay_ms(20); 
            if(!input(KEYPAD9)){
               val=7;
               switch(menu){
               case M_SPEED: speed=6; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=200; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD9)){
                  delay_ms(10);
               }
               delay_ms(50);
            }
         }
         output_float(KEYPAD5);
      }

      if(!slewing){
         //grey
         output_low(KEYPAD4);
         if(!input(KEYPAD6)){ // (8) I.C.
            delay_ms(20);
            if(!input(KEYPAD6)){
               val=8;
               switch(menu){
               case M_SPEED: speed=7; menu=M_BASE; break;
               case M_LIGHT: light_flag=1;light_pwm=255; menu=M_BASE; break;
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD6)){
                  delay_ms(10);
               }
               delay_ms(50);
            }
         }else if(!input(KEYPAD7)){ // (9) NGC
            delay_ms(20); 
            if(!input(KEYPAD7)){
               val=9;
               switch(menu){
               case M_MESSIER: enterNum(val); break;
               }
               while(!input(KEYPAD7)){
                  delay_ms(10);
               }
               delay_ms(50);
               
            }
         }else if(!input(KEYPAD8)){ // Goto
            delay_ms(20); 
            if(!input(KEYPAD8)){
               
               
            }
         }else if(!input(KEYPAD9)){ // ?
            delay_ms(20); 
            if(!input(KEYPAD9)){
               
            }
         }
         output_float(KEYPAD4);
      }

      if(!slewing){
         //blue
         output_low(KEYPAD3);
         if(!input(KEYPAD6)){ // Enter
            delay_ms(20);
            if(!input(KEYPAD6)){
               switch(menu){
               case M_BASE:
                  while(!input(KEYPAD6)){
                     blank();
                     text1="TiltSlew initiated ";
                     
                     
                     RA_out =0b11100000;
                     DEC_out=0b10100000;
                     
                     
                     set_adc_channel(0);
                     delay_ms(10);
                     adcX=read_adc();
                     delay_ms(10);
                     adcX+=read_adc();
                     delay_ms(10);
                     adcX+=read_adc();
                     delay_ms(10);
                     adcX+=read_adc();
                     delay_ms(10);
                     adcX+=read_adc();
                     
                     set_adc_channel(1);
                     delay_ms(10);
                     adcY=read_adc();
                     delay_ms(10);
                     adcY+=read_adc();
                     delay_ms(10);
                     adcY+=read_adc();
                     delay_ms(10);
                     adcY+=read_adc();
                     delay_ms(10);
                     adcY+=read_adc();
                     
                     gX=-(adcX-2162);
                     gY=adcY-2062;
                     
                     sprintf(text2, "%05ld %05ld", gX, gY);
                     
                     if(gX<-440){//east
                        text2[12]='-';
                        text2[13]='4';
                        RA_out=0b11001000 + 5;
                     }else if(gX<-330){
                        text2[12]='-';
                        text2[13]='3';
                        RA_out=0b11001000 + 4;
                     }else if(gX<-220){
                        text2[12]='-';
                        text2[13]='2';
                        RA_out=0b11001000 + 3;
                     }else if(gX<-110){
                        text2[12]='-';
                        text2[13]='1';
                        RA_out=0b11001000;
                     }else if(gX>440){//west
                        text2[12]='+';
                        text2[13]='4';
                        RA_out=0b11101000 + 5;
                     }else if(gX>330){
                        text2[12]='+';
                        text2[13]='3';
                        RA_out=0b11101000 + 4;
                     }else if(gX>220){
                        text2[12]='+';
                        text2[13]='2';
                        RA_out=0b11101000 + 3;
                     }else if(gX>110){
                        text2[12]='+';
                        text2[13]='1';
                        RA_out=0b11101000;
                     }else{
                        text2[12]=' ';
                        text2[13]='0';
                        RA_out=0b11100000;;
                     }
                     
                     if(gY<-440){//south
                        text2[15]='-';
                        text2[16]='4';
                        DEC_out=0b10001000 + 5;
                     }else if(gY<-330){
                        text2[15]='-';
                        text2[16]='3';
                        DEC_out=0b10001000 + 4;
                     }else if(gY<-220){
                        text2[15]='-';
                        text2[16]='2';
                        DEC_out=0b10001000 + 3;
                     }else if(gY<-110){
                        text2[15]='-';
                        text2[16]='1';
                        DEC_out=0b10001000;
                     }else if(gY>440){//north
                        text2[15]='+';
                        text2[16]='4';
                        DEC_out=0b10101000 + 5;
                     }else if(gY>330){
                        text2[15]='+';
                        text2[16]='3';
                        DEC_out=0b10101000 + 4;
                     }else if(gY>220){
                        text2[15]='+';
                        text2[16]='2';
                        DEC_out=0b10101000 + 3;
                     }else if(gY>110){
                        text2[15]='+';
                        text2[16]='1';
                        DEC_out=0b10101000;
                     }else{
                        text2[15]=' ';
                        text2[16]='0';
                        DEC_out=0b10100000;
                     }
                     
                     WriteLCD();
                 
                     //Send out RA/Dec commands (but don't repeat)
                     if(RA_out!=lastRA_out){
                        fputc('#', controller);
                        fputc('s', controller);
                        fputc(RA_out, controller);
                        delay_ms(50);
                     }
                     if(DEC_out!=lastDEC_out){
                        fputc('#', controller);
                        fputc('s', controller);
                        fputc(DEC_out, controller);
                        delay_ms(50);
                     }
                     
                     lastRA_out=RA_out;
                     lastDEC_out=DEC_out;
                  }
                  break;
               case M_SOLARSYSTEM:
                  sendTarget=1;
                  sentTarget=0;
                  menu = M_TARGET;
                  break;
               case M_MESSIER:
                  sendTarget=2;
                  sentTarget=0;
                  menu = M_TARGET;
                  break;
               }
               delay_ms(10);
            }
         }else if(!input(KEYPAD7)){ // Mode
            delay_ms(20); 
            if(!input(KEYPAD7)){
               if(menu>M_BASE){
                  menu=M_BASE;      
               }else{
                  if(tracking){
                     tracking=0;
                     RA_out=0b01000000;
                  }else{
                     tracking=1;
                     RA_out=0b01000001;
                  }
               }
               while(!input(KEYPAD7)){
                  delay_ms(1);
               }
               delay_ms(100);
            }
         }else if(!input(KEYPAD8)){ // Arrow up
            delay_ms(20); 
            if(!input(KEYPAD8)){
               switch(menu){
               case M_FOCUS: Focus_out=1; break;
               case M_SOLARSYSTEM:
                  if(solarSystem>0){
                     solarSystem--;
                  }
                  while(!input(KEYPAD8)){
                     delay_ms(1);
                  }
                  delay_ms(50);
                  break;
               }
              
            }
         }else if(!input(KEYPAD9)){ // Arrow down
            delay_ms(20); 
            if(!input(KEYPAD9)){
               switch(menu){
               case M_FOCUS: Focus_out=2; break;
               case M_SOLARSYSTEM:
                 if(solarSystem<8){
                     solarSystem++;
                  }
                  while(!input(KEYPAD9)){
                     delay_ms(1);
                  }
                  delay_ms(50);
                  break;
               }

            }
         }
         output_float(KEYPAD3);
      }

      output_float(KEYPAD1);
      output_float(KEYPAD2);
      output_float(KEYPAD3);
      output_float(KEYPAD4);
      output_float(KEYPAD5);
      
      if(light_flag){
         fputc('#', controller);
         fputc('l', controller);
         fputc(light_pwm, controller);
      
         light_flag=0;
      }

      blank();
      
      switch(menu){
      case M_BASE:
         if(slewing){
            sprintf(text1, "Slewing: %u %u", RA_out, DEC_out);
         }else if(tracking){
            text1="Tracking: Sidereal ";
         }else{
            text1="Tracking: Off      ";
         }
         sprintf(text2, "Slew speed: %u", speed+1);
         break;
      case M_SPEED:
         text1="Select slew speed: ";
         text2="1-8                ";
         break;
      case M_LIGHT:
         text1="Select light level:";
         text2="0-8                ";
         break;
      case M_FOCUS:
         if(Focus_out==2){
            text1="Focus              ";
            text2="Moving inwards     ";
         }else if(Focus_out==1){
            text1="Focus              ";
            text2="Moving outwards    ";
         }else{
            text1="Focus using bottom ";
            text2="arrow buttons      ";
         }
         break;
      case M_SOLARSYSTEM:
         text1="Solar system:      ";
         strcpy(text2, solarSystemStrings[solarSystem]);
         break;
      case M_MESSIER:   
         messier=numin[2]*100+numin[1]*10+numin[0];
      
         text1="Messier object:      ";
         sprintf(text2, "M%u", messier);
         break;
      case M_TARGET:
         text1="Target selected:   ";
         switch(sendTarget){
         case 1: strcpy(text2, solarSystemStrings[solarSystem]); break;
         case 2: sprintf(text2, "Messier %u", messier); break;
         }
         break;
      }
      
      WriteLCD();
      
      if(sendTarget && !sentTarget){
         fputc('#', controller);
         fputc('g', controller);
         fputc(sendTarget, controller);
         switch(sendTarget){
         case 1: fputc(solarSystem, controller); break;
         case 2: fputc(messier, controller); break;
         }   
         sentTarget=1;
      }
      
      
      //Send out RA/Dec commands (but don't repeat)
      if(RA_out!=lastRA_out){
         fputc('#', controller);
         fputc('s', controller);
         fputc(RA_out, controller);
         delay_ms(50);
      }
      if(DEC_out!=lastDEC_out){
         fputc('#', controller);
         fputc('s', controller);
         fputc(DEC_out, controller);
         delay_ms(50);
      }
      if(Focus_out!=lastFocus_out){
         fputc('#', controller);
         fputc('f', controller);
         fputc(Focus_out, controller);
         delay_ms(50);
      }
      
      lastRA_out=RA_out;
      lastDEC_out=DEC_out;
      lastFocus_out=Focus_out;
   }
}
