import numpy as np




file = open("rainfall.txt", "r")

regions = open("regions.txt", "r")

regionsList = []
regionFiles = []

regionData =  dict()
avg = dict()

def nextDate(date: str):
    components = date.split("/")
    if components[0] == "12":
        nextDate = "01/" + str(int(components[1])+1)
    elif components[0] == "09" or components[0] == "10" or components[0] == "11":
        nextDate = str(int(components[0])+1) + "/" + components[1]
    else:
        nextDate = "0" + str(int(components[0])+1) + "/" +components[1]
    return nextDate


for i in range(16):
    region = regions.readline().split(",")[1].strip(", \n")
    regionsList.append(region)
    regionData[region] = []
    avg[region] = 0
    regionFile = open(regionsList[i] + ".txt", "w")
    regionFiles.append(regionFile)
    regionFile.close()

counter = 0
date = "1836"

for line in file:
    
    line = line.split()
    region = regionsList[int(line[1])-1]
    
    regionFile = open(region + ".txt", "a")
    toWrite = date + "," + line[2] + "\n"
    regionFile.write(toWrite)
    regionFile.close()
    if int(date) > 2016:
        avg[region] += float(line[2])
    counter += 1
    if counter == 16:
        date = str(int(date) + 1)
        counter = 0


for region in avg.keys():
    print(region)
    print(avg[region]/5)

from random import random
import numpy as np

regions = open("regions.txt", "r")

regionAvgs = [734.634509103005,
              623.832981038109,
              1176.4293542873406,
              672.9687212252186,
              887.0620150333441,
              1675.501438777502,
              1362.972047525262,
              790.7516170561648,
              1083.7652761768156,
              790.6960972783015,
              1726.98618987606,
              897.6131318864989,
              0,
              1384.1499995547135,
              1160.4837004303301,
              1520.5587862664493]
regionPops = [48800944,
              6348096,
              2423400,
              8796628,
              2646772,
              378050,
              7422295,
              9294023,
              5712840,
              5954240,
              2420650,
              5481431,
              0,
              83314,
              1904563,
              3105410]
perPersonUsage = 102000000000000/(67330000) #in litres per year, using WWF water footprint and wikipedia uk population data
regionArea = [15627,
              12998,
              26570,
              1572,
              8579,
              27050,
              14108,
              19072,
              23836,
              12998,
              25670,
              15405,
              0,
              572,
              14130,
              20779] #filling in area data into array to calculate volume of water, then need a loss factor
regionsList = []
regionFiles = []
regionUsageFiles = []


sinRange = np.arange(12)
rainGen = dict()
usageGen = dict()
supplyGen = dict()

def noise():
    return (0.9 + random() * 0.2)
    


rainTotal = []

for i in range(16):
    rainTotal.append(regionArea[i]*1000000*regionAvgs[i])
    

rainTotalTotal = sum(rainTotal)
scaleFactor = 102000000000000/rainTotalTotal
print(scaleFactor)

for i in range(16):
    
    region = regions.readline().split(",")[1]
    regionsList.append(region.strip(", \n"))
    region = regionsList[i]
    
    if i != 12:


        rainGen[region] = [((np.cos((x/6)*np.pi)*0.5*regionAvgs[i] + regionAvgs[i]) * noise())/12 for x in sinRange]

        
        supplyGen[region] = [(x*scaleFactor*regionArea[i]*1000000) for x in rainGen[region]]
            
        usageGen[region] = [(x*perPersonUsage * regionPops[i])/12 for x in [noise()] * 12]
        

        
        regionFile = open(regionsList[i] + "Rainfall.txt", "w")
        regionUsageFile = open(regionsList[i] + "Usage.txt", "w")

        for x in rainGen[region]:
            regionFile.write(str(x)+"\n")
            

        for j in range(12):
            plusminus = supplyGen[region][j]/usageGen[region][j]-1

            regionUsageFile.write(str(plusminus*supplyGen[region][j])+"\n")
            

        #for x in supplyGen[region]:
        #    regionUsageFile.write(str(x)+"\n")
        
        regionFiles.append(regionFile)
        regionUsageFiles.append(regionUsageFile)
        regionFile.close()
        regionUsageFile.close()
    
    
    
