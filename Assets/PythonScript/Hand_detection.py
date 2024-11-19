import cv2
import numpy as np
import sys
import time  # To measure time

def count_spaces(binary_image):
    contours, _ = cv2.findContours(binary_image, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    if len(contours) == 0:
        return 0

    max_contour = max(contours, key=cv2.contourArea)
    
    if cv2.contourArea(max_contour) < 5000:
        return 0

    hull = cv2.convexHull(max_contour, returnPoints=False)
    if len(hull) < 3:
        return 0
    
    defects = cv2.convexityDefects(max_contour, hull)
    if defects is None:
        return 0

    count = 0
    for i in range(defects.shape[0]):
        s, e, f, d = defects[i, 0]
        start = tuple(max_contour[s][0])
        end = tuple(max_contour[e][0])
        far = tuple(max_contour[f][0])

        a = np.linalg.norm(np.array(end) - np.array(start))
        b = np.linalg.norm(np.array(far) - np.array(start))
        c = np.linalg.norm(np.array(end) - np.array(far))
        
        angle = np.arccos((b ** 2 + c ** 2 - a ** 2) / (2 * b * c))

        if angle <= np.pi * 105 / 180 and d > 12000:
            count += 1

    return count + 1

# Use DroidCam stream
cap = cv2.VideoCapture('http://172.20.10.13:4747/video')

# Variables to track consistent detection
consistent_sign = None
consistent_start_time = None
confirmation_time = 3  # seconds required for confirmation

while True:
    ret, frame = cap.read()
    if not ret:
        break

    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    lower_skin = np.array([0, 10, 120], dtype=np.uint8)
    upper_skin = np.array([20, 150, 255], dtype=np.uint8)
    mask = cv2.inRange(hsv, lower_skin, upper_skin)

    blurred = cv2.GaussianBlur(mask, (7, 7), 0)

    kernel = np.ones((5, 5), np.uint8)
    binary = cv2.morphologyEx(blurred, cv2.MORPH_CLOSE, kernel)
    binary = cv2.morphologyEx(binary, cv2.MORPH_OPEN, kernel)

    num_labels, labels, stats, centroids = cv2.connectedComponentsWithStats(binary, connectivity=8)
    
    if num_labels > 1:
        largest_label = 1 + np.argmax(stats[1:, cv2.CC_STAT_AREA])
        binary = (labels == largest_label).astype("uint8") * 255
    else:
        binary = np.zeros_like(binary)

    fingers = count_spaces(binary)
    
    # Determine the action based on the number of fingers detected
    action_text = ""
    if fingers == 2:
        action_text = "DoubleJump"
    elif fingers == 3:
        action_text = "Dash"
    elif fingers == 5:
        action_text = "Smash"

    # Check if the same action is detected consistently
    if action_text:
        if consistent_sign == action_text:
            # If already tracking this sign, check the time
            if time.time() - consistent_start_time >= confirmation_time:
                print(action_text)  # Send the confirmed action to Unity
                sys.stdout.flush()
                break
        else:
            # New sign detected, start tracking time
            consistent_sign = action_text
            consistent_start_time = time.time()
    else:
        # Reset if no valid action detected
        consistent_sign = None
        consistent_start_time = None

    # Add information to the frame for debugging (optional)
    cv2.putText(frame, f'Fingers: {fingers}', (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
    if consistent_sign:
        cv2.putText(frame, f'Hold: {consistent_sign}', (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

    cv2.imshow("Frame", frame)
    cv2.imshow("Binary", binary)

    # Exit loop on 'q' key press
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
