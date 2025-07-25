# RollingFruits
Rolling Fruits is the 3D platformer game where the player has to survive on conveyor belt for one minute.
The conveyor belts keeps moving the player backward and Fruits roll on the platform which pushes away the player.
The game becomes difficult as time passes.

- 움직이는 컨베이어 벨트 위에서 과일을 피하며 1분동안 살아남는 3D 플랫포머 게임입니다.
- 벨트는 계속해서 플레이어를 뒤로 밀어내며, 과일에 부딪히면 플레이어는 멀리 날아갑니다.
- 오래 버틸 수록, 게임의 난이도가 올라갑니다.

# Demo Video

https://github.com/user-attachments/assets/b69aa30b-3d15-45a5-9163-28e9c81c5bbc

[Video Link](https://youtu.be/eIDyDBptTEA?si=OV7CP1JOMD-wfgIz)

# Backtround Story
One day, the Mannequin saw the fashion shoot of the animals. 
There were rabbit which weared a twid jacket, dog with a trenchcoat, parrot with see-through one-piece and eagle with fine suit.
He wanted to get a new cloth, but he had a difficult financial situation.

On the next of the shoot, there was a post that looking for Fashion Model without any experience on the runway. He deciede to apply for his first career as a model.

Please help this mannequin finish the runway successfully without felling down for a one minute!

<img width="758" height="259" alt="image" src="https://github.com/user-attachments/assets/7b683e14-3c2a-48a7-a7a9-36773eb4a104" />

- 어느날, 마네킹은 동물들이 찍은 패션 화보를 보게 됩니다.
- 트위드 자켓을 입은 토끼, 트렌치 코트를 입은 강아지, 시스루 원피스를 입은 앵무새 그리고 수트를 입은 독수리를 보게 되었습니다.
- 마네킹은 새 옷을 갖고 싶었지만, 주머니 사정이 좋지 않았습니다.
- 화보 사진 옆에는 공고가 있었는데, 런웨이에 설 모델을 찾는다는 내용이었습니다.
- 경력 무관이라는 공고에 혹해서 마네킹은 첫 런웨이에 서기로 결심합니다.

  
# Source Code
## Player Control
### Handle Obstacle Collision

<img width="742" height="452" alt="image" src="https://github.com/user-attachments/assets/24fd6649-3607-4db7-95c6-c8bb9fab704b" />

1. Calculate the Player-Relative Force.
This is the direction between the fruit's forward direction and the player's position.
The result vector is on the XZ plane.

- 플레이어의 상대 위치를 정의합니다.
- 이 방향은, 공의 진행방향과 플레이어의 위치에 따라 플레이어가 밀려날 방향을 XZ plane에서 정의합니다.

  https://github.com/Sonak0930/RollingFruits/blob/d31bf915e32c723f9fe10f21796cb9efd51e94ae/Assets/Scripts/PlayerController.cs#L154


<img width="634" height="468" alt="image" src="https://github.com/user-attachments/assets/cfb5d900-07f0-4622-9b47-9f9570fcd959" />

2. Angular Knockback Force
This represents the actual knockback force in YZ Direction.
Z represents the horizontal Component, which is used for both XZ.
Y determines the height of the force.

- 넉백 힘을 정의합니다. 가로방향과 세로방향에서 각각 밀려날 힘의 크기를 정의합니다.

https://github.com/Sonak0930/RollingFruits/blob/d31bf915e32c723f9fe10f21796cb9efd51e94ae/Assets/Scripts/PlayerController.cs#L155-L160

<img width="1124" height="613" alt="image" src="https://github.com/user-attachments/assets/75e7a883-5722-4502-8743-c2b1ef8dd5bd" />

3. Combine the two forces into one
- 두 힘을 합해 최종 넉백을 정의합니다.


https://github.com/Sonak0930/RollingFruits/blob/d31bf915e32c723f9fe10f21796cb9efd51e94ae/Assets/Scripts/PlayerController.cs#L162-L165



