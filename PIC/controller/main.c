#include <main.h>



int32 indexra, indexdec;
int16 encra, encdec, focusposread;
int16 last_encra=0, last_encdec=0;

int8 debug1, debug2;

int8 blinking=0;
int1 blink_flag=0;

int8 command1, command2;
int8 RA_command=0;
int8 DEC_command=0;

int1  was_fast_moving=0, fast_moving_flag=0, fast_moving_flag_ra=0, fast_moving_flag_dec=0;

int16 light_pwm=512;
int1 light_flag=0;

float ha, dec, alt, az;

int1 tracking=0;
int1 tracking_flag=0;
int8 reply;
int8 eepval;

int8 tracking_status=0;

int1 tracking_request_flag=0;

int1 eeprom_reply_flag=0;
int8 eeprom_reply;

int8 k=0;

int1 lcd_flag=0;

int8 target1=0;
int8 target2=0;

int1 target_flag=0;

int1 motor_flag=0;

int8 focusmove=0;

int8 msb, lsb;

int8 RA_eeprom_flag=0;
int8 DEC_eeprom_flag=0;

int1 RA_enc_flag=0;
int1 DEC_enc_flag=0;

int1 focus_read_flag=0;
int1 focus_move_flag=0;

int8 datain1[4];
int8 datain2[4];

int8 RA_status;
int8 DEC_status;

int1 worm_read_flag=0;

int8 worm_index=0;

int16 i;

/*
float sin(float in){

   float p1, out;
   
   if(in>180){
      p1=in-180;
   }else{
      p1=in;
   }

   float p2=(4*p1*(180-p1))/(40500-p1*(180-p1));

   if(in>180){
      out=0-p1;
   }else{
      out=p1;
   }
   
   return out;

}

float cos(float in){

   float p1, out;
   
   if(in>360){in-=360;}
   
   if(in>270){
      out=-0.00000063*in*in*in-0.00001739*in*in+0.01774518*in;
   }else if(in>180){
      out=-0.00000063*in*in*in-0.00001739*in*in+0.01774518*in;
   }else if(in>90){
      out=-0.00000063*in*in*in-0.00001739*in*in+0.01774518*in;
   }else{
      out=-0.00000063*in*in*in-0.00001739*in*in+0.01774518*in;
   }
   
   return out;

}


float asin(float in){

   return in;  
   
}
   
*/
//-----------------------Constants

//Initial LCD display
char text1[21]="Marsh Telescope     ";
char text2[21]="Control Panel       ";

//LCD
const char Slave = 0x78;
const char Comsend = 0x00;
const char Datasend = 0x40;

const int16 fast_thresh=5;

const char Line1 = 0x80;
const char Line2 = 0xC0;

int8 inbytes[4];

int16 uabs(int16 num1, int16 num2){
   if(num1>num2){
      return (num1-num2);
   }else{
      return (num2-num1);
   }
}

void nextline(void)
{
   //Go to next line of LCD
   i2c_start(lcd);
   i2c_write(lcd, Slave);
   i2c_write(lcd, Comsend);
   i2c_write(lcd, Line2);
   i2c_stop(lcd);
}

//Clear LCD
void blank(void){
   text1="                   ";
   text2="                   ";
}

void firstline(void)
{
   //Go to first line of LCD
   i2c_start(lcd);
   i2c_write(lcd, Slave);
   i2c_write(lcd, Comsend);
   i2c_write(lcd, Line1);
   i2c_stop(lcd);
}

void Show(unsigned char *text)
{
   //Print *text to LCD display
   int n;//,d;
   //d=0x00;
   i2c_start(lcd);
   i2c_write(lcd, Slave);
   i2c_write(lcd, Datasend);
   for(n=0;n<20;n++){
      i2c_write(lcd, *text);
      ++text;
   }
   i2c_stop(lcd);
}

void encoderRA(){
   //get RA encoder value
   i2c_start(motor);
   i2c_write(motor, RA_ADD); //RA address
   i2c_write(motor, 'E'); //E = get encoder value
   delay_us(5);   //Give it some time to think
   i2c_start(motor);
   i2c_write(motor, RA_ADD|1);   //Get reply
   delay_us(10);
   inbytes[0] = i2c_read(motor, 1);
   delay_us(10);
   inbytes[1] = i2c_read(motor, 0);
   i2c_stop(motor);
   encra=make16(inbytes[1], inbytes[0]); //Make 16-bit number
   if(uabs(encra,last_encra)>fast_thresh){
      fast_moving_flag_ra=1;
   }else{
      fast_moving_flag_ra=0;
   }
   last_encra=encra;
}

void encoderDEC(){
   //get DEC encoder value
   i2c_start(motor);
   i2c_write(motor, DEC_ADD); //Dec address
   i2c_write(motor, 'E'); //E = get encoder value
   delay_us(5);   //Give it some time to think
   i2c_start(motor);
   i2c_write(motor, DEC_ADD|1);
   delay_us(10);
   inbytes[0] = i2c_read(motor, 1);//lsb
   delay_us(10);
   inbytes[1] = i2c_read(motor, 0);
   i2c_stop(motor);
   encdec=make16(inbytes[1], inbytes[0]); //Make 16-bit number  
   if(uabs(encdec,last_encdec)>fast_thresh){
      fast_moving_flag_dec=1;
   }else{
      fast_moving_flag_dec=0;
   }
   last_encdec=encdec;
   debug1= inbytes[0];//lsb
   debug2= inbytes[1];
}


void getRA_status(){
   //get RA encoder value
   i2c_start(motor);
   i2c_write(motor, RA_ADD); //RA address
   i2c_write(motor, 'S'); //S = get slew value
   delay_us(5);   //Give it some time to think
   i2c_start(motor);
   i2c_write(motor, RA_ADD|1);   //Get reply
   delay_us(10);
   RA_status = i2c_read(motor, 0);
   i2c_stop(motor);
}

void getDEC_status(){
   //get DEC status
   i2c_start(motor);
   i2c_write(motor, DEC_ADD); //RA address
   i2c_write(motor, 'S'); //S = get slew value
   delay_us(5);   //Give it some time to think
   i2c_start(motor);
   i2c_write(motor, DEC_ADD|1);   //Get reply
   delay_us(10);
   DEC_status = i2c_read(motor, 0);
   i2c_stop(motor);
}

void getTracking_status(){
   //get tracking status
   i2c_start(motor);
   i2c_write(motor, RA_ADD); //RA address
   i2c_write(motor, 'T'); //T =get track status
   delay_us(5);   //Give it some time to think
   i2c_start(motor);
   i2c_write(motor, RA_ADD|1);   //Get reply
   delay_us(10);
   tracking_status = i2c_read(motor, 0);
   i2c_stop(motor);
}

void  getWorm_index(){
   //get worm index
   i2c_start(motor);
   i2c_write(motor, RA_ADD); //RA address
   i2c_write(motor, 'W'); //W = get worm index
   delay_us(5);   //Give it some time to think
   i2c_start(motor);
   i2c_write(motor, RA_ADD|1);   //Get reply
   delay_us(10);
   worm_index = i2c_read(motor, 0);
   i2c_stop(motor);



}


#int_TIMER0
void  TIMER0_isr(void) {

   //15.2588Hz
   
   motor_flag=1;
   
   if(k){   
      lcd_flag=1;
      k=0;
   }else{
      k++;
   }
   
   blinking++;
   blink_flag=1;
   
   
   
}

//RS232 from handset
#int_RDA 
void  RDA_isr(void) {

   int8 in = fgetc(handset);

   if(in=='#'){

      int8 instruction = fgetc(handset);
      
      switch(instruction){
      case 's':
         //Slewing command
         command1 = fgetc(handset);

         if(command1==0x40){
            //request tracking off
            tracking=0;
            tracking_flag=1;
            //RA_slew_flag=1;
         }else if(command1==0x41){
            //request tracking on
            tracking=1;
            tracking_flag=1;
            //RA_slew_flag=1;
         }else if(bit_test (command1, 6)){
            //RA
            RA_command=command1;
            //RA_slew_flag=1;
         }else{
            //Dec
            DEC_command=command1;
            //DEC_slew_flag=1;
         }         
         break;
      case 'f':
         //Move focuser
         command1 = fgetc(handset);
         focusmove=command1;
         focus_move_flag=1;
         
         break;
      case 'l':
         //Light command
         light_pwm=make16(inbytes[1], fgetc(handset))*4;
         light_flag=1;
         break;
      case 't':
         //target set
         target1=fgetc(handset);
         target2=fgetc(handset);
         target_flag=1;
         break;
      }
   }else if(in=='$'){

      int8 instruction = fgetc(handset);
      
      switch(instruction){
         case 'R':
         //eeprom val being returned
         eeprom_reply=fgetc(handset);
         eeprom_reply_flag=1;
         break;
      }
      
   }else{
      //clear buffer
      //while(kbhit(handset)){
         //fgetc(handset);
      //}   
   }
}

//RS232 from PC
#int_RDA2 
void  RDA2_isr(void) {

   int8 in = fgetc(pc);

   if(in=='#'){
      in = fgetc(pc);
      switch(in){
      case 'H':
         //Request HA encoder
         RA_enc_flag=1;
         break;
      case 'D':
         //Request Dec encoder         
         DEC_enc_flag=1;
         break;
      case 'S':
         //Get track/slew state
         tracking_request_flag=1;
         break;
      case 'F':
         //get focuser position
         focus_read_flag=1;
         break;
     case 'W':
         //get worm index
         worm_read_flag=1;
         break;
      case 'R':         
         //read eeprom
         datain1[0]=fgetc(pc);//chip
         datain1[1]=fgetc(pc);//address msb
         datain1[2]=fgetc(pc);//address lsb
         
         switch(datain1[0]){
         case 0: //Controller
            fputc('$', pc);
            fputc('R', pc);
            fputc(read_eeprom((datain1[1]<<8) | (datain1[2])), pc);
            break;
         case 1: //Handset
            fputc('#', handset);
            fputc('R', handset);
            fputc(datain1[1], handset);
            fputc(datain1[2], handset);
            break;
         case 2: //RA
            RA_eeprom_flag=1;
            break;
         case 3: //DEC
            DEC_eeprom_flag=1;      
            break;
         }         
         break;
      case 's':
         //Set tracking/slewing
         command2 = fgetc(pc);

         if(command2==0x40){
            //request tracking off
            tracking=0;
            tracking_flag=1;
            //Let the handset know what's happening
            fputc('#', handset);
            fputc('t', handset);
            fputc(command2, handset);
            
         }else if(command2==0x41){
            //request tracking on
            tracking=1;
            tracking_flag=1;
            //Let the handset know what's happening
            fputc('#', handset);
            fputc('t', handset);
            fputc(command2, handset);
            
         }else if(bit_test (command2, 6)){
            //RA
            RA_command=command1;
         }else{
            //Dec
            DEC_command=command1;
         }   
         break;
      case 'r':
         //Write eeprom
         if(!RA_eeprom_flag && !DEC_eeprom_flag){
            
            datain1[0]=fgetc(pc);//chip
            datain1[1]=fgetc(pc);//address msb
            datain1[2]=fgetc(pc);//address lsb
            datain1[3]=fgetc(pc);//data
            switch(datain1[0]){
            case 0: //Controller
               write_eeprom((datain1[1]<<8) | (datain1[2]), datain1[3]);
               break;
            case 1: //Handset
               fputc('#', handset);
               fputc('r', handset);
               fputc(datain1[1], handset);
               fputc(datain1[2], handset);
               fputc(datain1[3], handset);
               break;
            case 2: //RA
               RA_eeprom_flag=2;
               break;
            case 3: //DEC
               DEC_eeprom_flag=2;      
               break;
            }
         }
         break;
      case 'l':
         //Light command
         light_pwm=(int16)fgetc(pc)<<2;
         light_flag=1;
         break;
      }
   }else{
      //clear buffer
      //while(kbhit(handset)){
      //   fgetc(handset);
      //}
   }
}


//-----------------------Main function
void main(){
   output_low(FOCUSA);
   output_low(FOCUSB);
   
   //Setup analog ports, range 0-5v
   setup_adc_ports(sAN0|sAN1|VSS_VDD);
   setup_adc(ADC_CLOCK_DIV_64|ADC_TAD_MUL_16);
   set_adc_channel(FOCUSPOS);
   


   //WDT on
   setup_wdt(WDT_ON);

   //setup_spi(SPI_MASTER | SPI_H_TO_L | SPI_CLK_DIV_64);

   //Timer not used at the moment
   setup_timer_0(RTCC_INTERNAL|RTCC_DIV_16);      //  16=65ms, 32=131ms, 64=242ms
   setup_timer_3(T3_DISABLED|T3_DIV_BY_1);
   setup_timer_4(T4_DISABLED,0,1);
   setup_timer_6(T6_DIV_BY_16,255,1);
   
   setup_comparator(NC_NC_NC_NC);
   
   setup_ccp5(CCP_PWM|CCP_USE_TIMER5_AND_TIMER6);
     
   set_pwm5_duty(0);


   //Initialise the LCD
   i2c_start(lcd);
   if (i2c_write(lcd, slave)==0){
      i2c_write(lcd, Comsend);
      i2c_write(lcd, 0x38); //function set
      delay_ms(10);
      i2c_write(lcd, 0x39); //function set
      delay_ms(10);
      i2c_write(lcd, 0x14); //int osc freq
      i2c_write(lcd, 0x79); //contrast set
      i2c_write(lcd, 0x50); //power/icon/contrast set
      i2c_write(lcd, 0x6C); //follower control
      i2c_write(lcd, 0x0C); //display off
      i2c_write(lcd, 0x01); //clear display
      i2c_write(lcd, 0x06);
      delay_ms(10);
   }
   i2c_stop(lcd);
   
   //Show startup texxt
   firstline();
   Show(text1);
   nextline();
   Show(text2);

   restart_wdt();
   
   for(i=0; i<1024; i++){
      set_pwm5_duty(i);
      delay_ms(1);
   }
   
   
   
   text1="Loading...          ";
   text2="Please wait         ";
   
   firstline();
   Show(text1);
   nextline();
   Show(text2);

   restart_wdt();
   for(i=1023; i>20; i--){
      set_pwm5_duty(i);
      delay_ms(1);
   }
   light_pwm=20;

   //clear variables
   //int32 index=0;
   text1="                    ";
   text2="                    ";


   enable_interrupts(INT_TIMER0);
   //Data from handset
   enable_interrupts(INT_RDA);
   //Data from PC
   enable_interrupts(INT_RDA2);
   //Enable interrupts
   enable_interrupts(GLOBAL);
   
   

   while(1){
      //-----------------------------------------Main loop
      restart_wdt();

      //delay_ms(50);

      /*

      //get RA index
      i2c_start(motor);
      i2c_write(motor, RA_ADD);
      i2c_write(motor, 'i');
      delay_us(5);
      i2c_start(motor);
      i2c_write(motor, RA_ADD|1);
      delay_us(10);
         inbytes[0] = i2c_read(motor, 1);
      delay_us(10);
         inbytes[1] = i2c_read(motor, 1);
      delay_us(10);
         inbytes[2] = i2c_read(motor, 1);
      delay_us(10);
         inbytes[3] = i2c_read(motor, 0);
      i2c_stop(motor);
      */
      //indexra=make32(0,inbytes[2], inbytes[1], inbytes[0]);
      
      if(tracking_flag){ 
         //Send target from handset to PC
         i2c_start(motor);
         i2c_write(motor, RA_ADD); //RA address
         i2c_write(motor, 't'); //Track command
         i2c_write(motor, tracking);
         i2c_stop(motor); 
         
         tracking_flag=0;
      }
        
         
      if(motor_flag){     
         
         //delay_ms(5);
         focusposread=read_adc();
         
         //Read encoder
         encoderRA();
         encoderDEC();
         
         fast_moving_flag=(fast_moving_flag_ra || fast_moving_flag_dec);
         
         
         /*
         ha=360*(float)encra/16384;
         dec=360*(float)encdec/16384;
         
         float lat=51.775;
         
         alt=asin(sin(dec)*sin(lat)+cos(dec)*cos(lat)*cos(ha));
         */
         
         getRA_status();
         getDEC_status();
         
         getWorm_index();
         
         getTracking_status();
         
         //send RA command
         i2c_start(motor);
         i2c_write(motor, RA_ADD); //RA address
         i2c_write(motor, 's'); //Slew/track command
         i2c_write(motor, RA_command);
         i2c_stop(motor); 
            
         //send DEC command
         i2c_start(motor);
         i2c_write(motor, DEC_ADD); //Dec address
         i2c_write(motor, 's'); //Slew/track command
         i2c_write(motor, DEC_command);
         i2c_stop(motor); 
      
         motor_flag=0;
      }
         

      
      if(RA_enc_flag){         
         //Send RA enc to PC
      
         lsb=encra & 0xFF;
         msb=(encra>>8)& 0xFF;
         
         //reply to pc
         fputc('$', pc);
         fputc('H', pc);
         fputc(msb, pc);
         fputc(lsb, pc);  
         
         RA_enc_flag=0;
      }
      
      if(DEC_enc_flag){         
         //Send Dec enc to PC
      
         lsb=encdec & 0xFF;
         msb=(encdec>>8)& 0xFF;
         
         //reply to pc
         fputc('$', pc);
         fputc('D', pc);
         fputc(msb, pc);
         fputc(lsb, pc);  
         
         DEC_enc_flag=0;
      }
      
      if(worm_read_flag){
      
         //reply to pc
         fputc('$', pc);
         fputc('W', pc);
         fputc(worm_index, pc);
         worm_read_flag=0;
      
      }
      
      if(RA_eeprom_flag==1){
         //Write to RA eeprom   
         i2c_start(motor);
         i2c_write(motor, RA_ADD);
         i2c_write(motor, 'r'); //eeprom write
         i2c_write(motor, datain1[1]);
         i2c_write(motor, datain1[2]);
         i2c_write(motor, datain1[3]);
         i2c_stop(motor); 
         
         RA_eeprom_flag=0;
         
      }else if(RA_eeprom_flag==2){
         //read RA eeprom
         
         i2c_start(motor);
         i2c_write(motor, RA_ADD);
         i2c_write(motor, 'R'); //eeprom read
         delay_us(5);
         i2c_write(motor, datain1[1]);
         delay_us(5);
         i2c_write(motor, datain1[2]);
         delay_us(5);   //Give it some time to think
         i2c_start(motor);
         i2c_write(motor, RA_ADD|1);   //Get reply
         delay_us(10);
         eepval = i2c_read(motor, 0);
         i2c_stop(motor);
         
         fputc('$', pc);
         fputc('R', pc);
         fputc(eepval, pc);
         
         RA_eeprom_flag=0;
      }
      
      if(DEC_eeprom_flag==1){
         //Write to RA eeprom   
         i2c_start(motor);
         i2c_write(motor, DEC_ADD);
         i2c_write(motor, 'r'); //eeprom write
         i2c_write(motor, datain1[1]);
         i2c_write(motor, datain1[2]);
         i2c_write(motor, datain1[3]);
         i2c_stop(motor); 
         
         DEC_eeprom_flag=0;
         
      }else if(DEC_eeprom_flag==2){
         //read DEC eeprom
         
         i2c_start(motor);
         i2c_write(motor, DEC_ADD);
         i2c_write(motor, 'R');//eeprom read
         delay_us(5);
         i2c_write(motor, datain1[1]);
         delay_us(5);
         i2c_write(motor, datain1[2]);
         delay_us(5);   //Give it some time to think
         i2c_start(motor);
         i2c_write(motor, DEC_ADD|1);   //Get reply
         delay_us(10);
         eepval = i2c_read(motor, 0);
         i2c_stop(motor);
         
         fputc('$', pc);
         fputc('R', pc);
         fputc(eepval, pc);
         
         DEC_eeprom_flag=0;
      }
      
      if(focus_read_flag){
         //Report focus pos to PC       
         
         lsb=focusposread & 0xFF;
         msb=(focusposread>>8)& 0xFF;
         
         //reply to pc
         fputc('$', pc);
         fputc('F', pc);
         fputc(msb, pc);
         fputc(lsb, pc);  
         
         focus_read_flag=0;
      }
      
      if(focus_move_flag){
      
         if(focusmove==1){
            output_high(FOCUSA);
            output_low(FOCUSB);         
         }else if(focusmove==2){
            output_low(FOCUSA);
            output_high(FOCUSB);
         }else{
            output_low(FOCUSA);
            output_low(FOCUSB);
         }
      
         focus_move_flag=0;
      }
      
      if(eeprom_reply_flag){
         //Return eeprom reply from handset to PC
         fputc('$', pc);
         fputc('R', pc);
         fputc(eeprom_reply, pc);
         
         eeprom_reply_flag=0;
      }
      
      if(target_flag){ 
         //Send target from handset to PC
         fputc('#', pc);
         fputc('t', pc);
         fputc(target1, pc);
         fputc(target2, pc);
         
         target_flag=0;
      }
      
      if(tracking_request_flag){
         //Send tracking status to PC
         fputc('$', pc);
         fputc('S', pc);
         fputc(tracking, pc);
         tracking_request_flag=0;
      }
      
      if(fast_moving_flag && blink_flag){
         if((blinking/4)%2){
            set_pwm5_duty((int16)100);
         }else{
            set_pwm5_duty(light_pwm);
         }
          blink_flag=0;
         
      }else if(fast_moving_flag!=was_fast_moving){
         set_pwm5_duty(light_pwm);
      
      }else if(light_flag){
         set_pwm5_duty(light_pwm);
      
         light_flag=0;
      }
       was_fast_moving=fast_moving_flag;
      
      if(lcd_flag){
         blank();
          //Draw control panel LCD
         sprintf(text1, "%05ld %05ld %u%u%u|%u%u%u", encra, encdec, (int8)tracking, tracking_status, focusmove, (int8)fast_moving_flag_ra, (int8)fast_moving_flag_dec, (int8)fast_moving_flag); 
         sprintf(text2, "%u %u %ld %u", RA_command, DEC_command, focusposread, worm_index); // inbytes[1], inbytes[0],
      
         //delay_ms(10);
         firstline();
         Show(text1);
         nextline();
         Show(text2);
         
         lcd_flag=0;
      }
      
   }

}
