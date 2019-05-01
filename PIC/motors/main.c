#include <main.h>


//----------------------Globals
int32 index=0;
int32 speed=0;
int8 test=0;
int16 ShaftEnc;
int1 newDir=1, Dir=1, En=0;

int8 slewset=0;
int8 last_slewset=0;

int8 worm_index=0;


int1 istracking=0;

//#BYTE SSPBUFF=0xFC9
//#BYTE SSPCON1=0xFC6
//#bit SBIT=SSPCON1.4

//int8 I2C_BUFF[4];
int8 commandin=0;
int8 bytes[4];
int8 datain[4];
//----------------------Here be the interrupts
/*
#int_TIMER0
void  TIMER0_isr(void) 
{

}

#int_TIMER1
void  TIMER1_isr(void) 
{

}

#int_TIMER2
void  TIMER2_isr(void) 
{

}

#int_TIMER3
void  TIMER3_isr(void) 
{

}
*/
#int_EXT
void  EXT_isr(void) 
{
   if(Dir){
      index++;
   }else{
      index--;
   }

}
/*
#int_EXT1
void  EXT1_isr(void) 
{

}

#int_EXT2
void  EXT2_isr(void) 
{

}

#int_RB
void  RB_isr(void) 
{

}
*/

void getWormIndex(){


  // worm_index=(input(PIN_D7)<<7)+(input(PIN_D6)<<6)+(input(PIN_D5)<<5)+(input(PIN_D4)<<4)+(input(PIN_C5)<<3)+(input(PIN_C4)<<2)+(input(PIN_D3)<<1)+input(PIN_D2);
   
   worm_index=0;

   if(input(WORM7)){bit_set(worm_index,7);}
   if(input(WORM6)){bit_set(worm_index,6);}
   if(input(WORM5)){bit_set(worm_index,5);}
   if(input(WORM4)){bit_set(worm_index,4);}
   if(input(WORM3)){bit_set(worm_index,3);}
   if(input(WORM2)){bit_set(worm_index,2);}
   if(input(WORM1)){bit_set(worm_index,1);} 
   if(input(WORM0)){bit_set(worm_index,0);}
   
}

#int_SSP2
void  SSP2_isr(void){

   int8 state = i2c_isr_state();

   switch (state) {
   case 0: //i2c address
      i2c_read(1);  
      //test++;
      break;
   case 1: //command
      commandin=i2c_read(1);
      break;
   case 2:
      datain[0]=i2c_read(1);
      if(commandin=='s'){
         slewset=datain[0];
      }else if(commandin=='t' && isRA){
         //newtracking=datain[0];
         if(datain[0]){
            istracking=1;
         }else{
            istracking=0;
         }
         //tracking_flag=1;
      }
      break;
   case 3:
      datain[1]=i2c_read(1);
      break;
   case 4:
      datain[2]=i2c_read(1);
      if(commandin=='r'){
         write_eeprom((datain[0]<<8) | (datain[1]), datain[2]);
      }
      break;
   case 5:
      datain[3]=i2c_read(0);
      break;
   case 0x80: // Master is requesting data from slave
      switch (commandin) {
      case 'E':
         //Requesting encoder value
         bytes[0] =ShaftEnc & 0xFF ;
         bytes[1] = (ShaftEnc >> 8) & 0xFF;
         break;
      case 'I':
         //requesting index value
         bytes[0] =index & 0xFF ;
         bytes[1] = (index >> 8) & 0xFF;
         bytes[2] = (index >> 16) & 0xFF;
         bytes[3] = (index >> 24) & 0xFF;
         break; 
      case 'R':
         bytes[0]=read_eeprom((datain[0]<<8) | (datain[1]));
         break;
      case 'S':
         bytes[0]=last_slewset;
         break;
      case 'T':
         bytes[0]=(int8)istracking;
         break;
      case 'W':
         getWormIndex();
         bytes[0]=worm_index;
         break;
      default:
         bytes[0] =0;
         bytes[1] =0;
         bytes[2] =0;
         bytes[3] =0;
         break;
      }
      i2c_write(bytes[0]);
      break;
   case 0x81:
      i2c_write(bytes[1]);
      break;
   case 0x82:
      i2c_write(bytes[2]);
      break;
   case 0x83:
      i2c_write(test);
      break;
   default:
      i2c_write(0); 
      break;
   }
   
} 

void setEn(int1 enabled){
   output_bit(DISABLED, !enabled);
   En=enabled;
}

void setDir(int1 direc){
   output_bit(DIRECTION, direc);
   Dir=direc;
}


//-----------------------Constants


int16 time;

//-----------------------Here be other functions


void freq2word(int32 number){
   int16 LSB, MSB;
   //get 14 LSBs of the 28 bit num
   LSB = number & 0b0011111111111111;
   //set control bits
   bit_set(LSB,14);
   number <<=2;
   number >>=16; //this seems odd, but takes 14 cycles instead of the 30 for >>=14
   MSB = number & 0b0011111111111111;
   bit_set(MSB,14);

   //SPI_16out(LSB); //LS bytes (MSB first in that)
   //SPI_16out(MSB); //MS bytes

   output_low(FSYNC);
   spi_xfer(SPI_1, LSB, 16); 
   spi_xfer(SPI_1, MSB, 16);  
   output_high(FSYNC);
}

int32 ramp(int32 target){
   int32 now=speed;
   if(target==now){
      return now;
   }else{
      int32 i;
      
      if(target>now){
         i=now+1000000;
         while(i<target){
            freq2word(i);
            i=i+1000000;
            delay_ms(18);
         }
         delay_ms(10);
         i=target;
         freq2word(target);
      }else if(target<now && now>1000000){
         i=now-1000000;
         while(i>target && i>1000000){
            freq2word(i);
            i=i-1000000;
            delay_ms(10);
         }
         delay_ms(10);
         i=target;
         freq2word(target);
         
      }else{
         i=target;
         freq2word(target);
      }
      return i;   
   }
}



//-----------------------Main
void main(){

   setEn(0);
   setDir(1);


   //---------------------- Peripheral setup

   
   //-----------Variables

   int8 eeprom[256];//, pec, handset, newspeed, encoder, encoderpre, count;
   //int8 in1, in2, in3, in4;



   int16 i;
   //int16 adcin, enctime;
   //char in;

   unsigned int32 speeds[8];
   Speed=0;

   //Set SPI fsyncs and load high
   output_high(FSYNC);
   output_high(LOAD);

   //Motors are disabled at startup



   port_b_pullups(TRUE);
   setup_adc_ports(sAN0|VSS_VDD);
   setup_adc(ADC_CLOCK_DIV_64|ADC_TAD_MUL_16);
   set_adc_channel(0); 
   
   //setup_spi2(SPI_SLAVE | SPI_H_TO_L);
   setup_wdt(WDT_ON);
   //setup_timer_0(RTCC_INTERNAL|RTCC_DIV_256);      //1.0 s overflow
   
   setup_timer_0(RTCC_INTERNAL|RTCC_DIV_1);      //4.0 ms overflow
   setup_timer_1(T1_INTERNAL|T1_DIV_BY_8);      //32.7 ms overflow
   setup_timer_2(T2_DIV_BY_16,207,13);      //208 us overflow, 2.7 ms interrupt
   setup_timer_3(T3_INTERNAL|T3_DIV_BY_8);
   setup_timer_4(T4_DIV_BY_16,255,16);


   ext_int_edge(L_TO_H);
   /*
   enable_interrupts(INT_TIMER0);
   enable_interrupts(INT_TIMER1);
   enable_interrupts(INT_TIMER2);
   enable_interrupts(INT_TIMER3);
   
   enable_interrupts(INT_EXT1);
   enable_interrupts(INT_EXT2);
   enable_interrupts(INT_RB);
   enable_interrupts(INT_SSP);
   */
   
   
   
   if(read_EEPROM(1022)!=128){
      //default values not set
      write_eeprom(1022, 128);
      write_eeprom(1023, 0);
      
      //sidereal/guide speed (0)
      write_eeprom(1, 0);
      write_eeprom(2, 12);
      write_eeprom(3, 43);
      write_eeprom(4, 66);
      
      //centre speed (1)
      write_eeprom(5, 0);
      write_eeprom(6, 48);
      write_eeprom(7, 173);
      write_eeprom(8, 8);
      
      //find speed (2)
      write_eeprom(9, 0);
      write_eeprom(10, 75);
      write_eeprom(11, 2);
      write_eeprom(12, 8);
      
      //slew speed (3)
      write_eeprom(13, 0);
      write_eeprom(14, 120);
      write_eeprom(15, 181);
      write_eeprom(16, 64);
      
      // (4)
      write_eeprom(17, 0);
      write_eeprom(18, 200);
      write_eeprom(19, 0);
      write_eeprom(20, 0);
      
      //find speed (5)
      write_eeprom(21, 1);
      write_eeprom(22, 49);
      write_eeprom(23, 45);
      write_eeprom(24, 0);
      
      //find speed 2 (6)
      write_eeprom(25, 3);
      write_eeprom(26, 185);
      write_eeprom(27, 172);
      write_eeprom(28, 160);
      
      //slew speed (7)
      write_eeprom(29, 7);
      write_eeprom(30, 115);
      write_eeprom(31, 89);
      write_eeprom(32, 64);

   }


   //Read entire eeprom into RAM for quicker access
   for (i = 0; i <256; i++){
      eeprom[i] = read_EEPROM(i);
   }

   //default speeds
   speeds[0]=make32(eeprom[1],eeprom[2],eeprom[3],eeprom[4]);
   speeds[1]=make32(eeprom[5],eeprom[6],eeprom[7],eeprom[8]);
   speeds[2]=make32(eeprom[9],eeprom[10],eeprom[11],eeprom[12]);
   speeds[3]=make32(eeprom[13],eeprom[14],eeprom[15],eeprom[16]);
   speeds[4]=make32(eeprom[17],eeprom[18],eeprom[19],eeprom[20]);
   speeds[5]=make32(eeprom[21],eeprom[22],eeprom[23],eeprom[24]);
   speeds[6]=make32(eeprom[25],eeprom[26],eeprom[27],eeprom[28]);
   speeds[7]=make32(eeprom[29],eeprom[30],eeprom[31],eeprom[32]);

   //Reset and setup signal generators



   output_low(FSYNC);
   spi_xfer(SPI_1, RESET, 16); 
   spi_xfer(SPI_1, 0b0100000000000000, 16); 
   spi_xfer(SPI_1, 0b0100000000000000, 16); 
   spi_xfer(SPI_1, 0b1100000000000000, 16); 
   spi_xfer(SPI_1, UNSET, 16);
   output_high(FSYNC);



   //encoderpre=0;

   //output_high(DISABLED);
  
   //En=1;

   //if(Dir!=1){
   //   ramp(0, Speed);
   //}
   //output_low(DISABLED);
   //output_high(DIRECTION);
   //Dir=1;
   //delay_us(2);   

   //speed=ramp(Speeds[0], 0);

   //delay_ms(500);

   //speed=ramp(speeds[8], speed);

   //istracking=1;
   
   delay_ms(10);

   enable_interrupts(INT_EXT);
   enable_interrupts(INT_SSP2); 
   enable_interrupts(GLOBAL);

   setEn(1);


   while(1){
      //-----------------------------------------Main loop
      restart_wdt();

      //read encoder
      output_low(LOAD);
      delay_us(1);
      output_high(LOAD);
      time = spi_xfer(SPI_1,0,16); 
      ShaftEnc=(~time)>>2;

     /* if(tracking_flag && isRA){
      
         if(newtracking)
            //turn tracking on
            if(!bit_test(slewset,3)){
               setDir(1);
               delay_us(3);
               speed=ramp(speeds[0]);
            }
            istracking=1;
         }else if(slewset==0x41){
            //turn tracking off
            setDir(1);
            delay_us(3);
            speed=ramp(0);
            istracking=0;
         }
         //slewset=0;
      
      }else */
      if(slewset){
         last_slewset=slewset;
         if(bit_test(slewset,3)){
            //being asked to slew
            newDir=bit_test(slewset,5);
            if((slewset & 7)==0 && istracking && isRA){
               //special case for guide speed if tracking
               if(newDir){
                  //west, double speed
                  setDir(newdir);
                  delay_us(3);
                  speed=ramp(speeds[0]*2);
               }else{
                  //east, stop motor
                  speed=ramp(0);
               }         
            }else{            
               if(newDir!=Dir){//Stop before changing direction
                  speed=ramp(0);
               }
               setDir(newdir);
               delay_us(3);
               speed=ramp(speeds[slewset & 7]);      
            }
            
         }else{
            //stopped being asked to slew, return to tracking or zero
            if(istracking && isRA){
               //return to tracking speed
               if(!Dir){
                  //going non-sidereal direction, stop before return 
                  speed=ramp(0);
                  setDir(1);
                  delay_us(3);
               }
               //tracking speed
               speed=ramp(speeds[0]);
            }else{
               //Not tracking, stop motor
               speed=ramp(0);
               delay_us(2);
               setDir(1);
            }
            
         }
         slewset=0;
      }

      delay_ms(10);
   }
}
