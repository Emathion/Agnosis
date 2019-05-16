import face_recognition
import glob
import os

os.chdir("C:\ProgramData\AttendanceSys\images")

known_images_names = []
known_images = []
known_encodings = []
 # this will be the database access
for file in glob.glob("*.png"):
  if file=="ImageToMark.png":
    continue
  known_images_names.append(file)
# print (known_images_names)
for file in known_images_names:
  known_images.append(face_recognition.load_image_file(file))
# print (known_images)
for filename in known_images:
  e = face_recognition.face_encodings(filename)
  if (len(e) > 0):
    known_encodings.append(face_recognition.face_encodings(filename)[0]) 
# print (known_encodings)
 #this will be the image received from c#
unknown_image1 = face_recognition.load_image_file("ImageToMark.png")

result = [False]
e = face_recognition.face_encodings(unknown_image1)
if (len(e) > 0):
  unknown_encoding1 = face_recognition.face_encodings(unknown_image1)[0]
  # print (unknown_encoding1)
  for x in known_encodings:
    result = face_recognition.compare_faces([x], unknown_encoding1)
    if (result[0]==True):
      print ("1")
      break

if (result[0]==False):
  print ("0")

