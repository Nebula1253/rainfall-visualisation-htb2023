from PIL import Image
import numpy as np
# 1. Read image
img = Image.open("C:\\Users\\Neel Amonkar\\OneDrive - University of Edinburgh\\Rain\\Rain\\Assets\\EnglandIrelandBWMap.jpg")
 
# 2. Convert image to NumPy array
arr = np.asarray(img)
print(arr.shape)
# (771, 771, 3)
# 3. Convert 3D array to 2D list of lists
lst = []
for row in arr:
    tmp = []
    for col in row:
        tmp.append(str(col))
    lst.append(tmp)
# 4. Save list of lists to CSV
with open("C:\\Users\\Neel Amonkar\\OneDrive - University of Edinburgh\\Rain\\Rain\\Assets\\CSVs\\ukregions.csv", 'w') as f:
    for row in lst:
        f.write(','.join(row) + '\n')