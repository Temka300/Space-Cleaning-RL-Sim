[INFO] Hyperparameters for behavior name SpaceCleaner: 
        trainer_type:   spacecleaner_ppo
        hyperparameters:
          batch_size:   2048
          buffer_size:  32768
          learning_rate:        0.00025
          beta: 0.0075
          epsilon:      0.15
          lambd:        0.95
          num_epoch:    3
          shared_critic:        False
          learning_rate_schedule:       linear
          beta_schedule:        linear
          epsilon_schedule:     linear
          action_smoothness_coef:       0.01
        checkpoint_interval:    250000
        network_settings:
          normalize:    True
          hidden_units: 256
          num_layers:   2
          vis_encode_type:      simple
          memory:
            sequence_length:    64
            memory_size:        128
          goal_conditioning_type:       hyper
          deterministic:        False
        reward_signals:
          extrinsic:
            gamma:      0.995
            strength:   1.0
            network_settings:
              normalize:        False
              hidden_units:     128
              num_layers:       2
              vis_encode_type:  simple
              memory:   None
              goal_conditioning_type:   hyper
              deterministic:    False
        init_path:      None
        keep_checkpoints:       5
        even_checkpoints:       False
        max_steps:      5000000
        time_horizon:   256
        summary_freq:   10000
        threaded:       True
        self_play:      None
        behavioral_cloning:     None
[INFO] Parameter 'difficulty_level' is in lesson 'single-target' and has value 'Float: value=0.0'.
[INFO] SpaceCleaner. Step: 10000. Time Elapsed: 75.925 s. Mean Reward: -2.082. Std of Reward: 2.548. Training.
[INFO] SpaceCleaner. Step: 20000. Time Elapsed: 128.183 s. Mean Reward: -2.674. Std of Reward: 0.703. Training.
[INFO] SpaceCleaner. Step: 30000. Time Elapsed: 184.131 s. Mean Reward: -2.885. Std of Reward: 0.715. Training.
[INFO] SpaceCleaner. Step: 40000. Time Elapsed: 232.992 s. Mean Reward: -2.708. Std of Reward: 0.694. Training.
[INFO] SpaceCleaner. Step: 50000. Time Elapsed: 284.474 s. Mean Reward: -2.571. Std of Reward: 0.615. Training.
[INFO] SpaceCleaner. Step: 60000. Time Elapsed: 337.420 s. Mean Reward: -2.546. Std of Reward: 0.661. Training.
[INFO] SpaceCleaner. Step: 70000. Time Elapsed: 388.020 s. Mean Reward: -2.383. Std of Reward: 1.318. Training.
[INFO] SpaceCleaner. Step: 80000. Time Elapsed: 438.043 s. Mean Reward: -2.471. Std of Reward: 0.501. Training.
[INFO] SpaceCleaner. Step: 90000. Time Elapsed: 493.480 s. Mean Reward: -2.320. Std of Reward: 1.474. Training.
[INFO] SpaceCleaner. Step: 100000. Time Elapsed: 542.375 s. Mean Reward: -2.562. Std of Reward: 0.486. Training.
[INFO] SpaceCleaner. Step: 110000. Time Elapsed: 592.568 s. Mean Reward: -2.256. Std of Reward: 1.396. Training.
[INFO] SpaceCleaner. Step: 120000. Time Elapsed: 643.893 s. Mean Reward: -2.342. Std of Reward: 0.556. Training.
[INFO] SpaceCleaner. Step: 130000. Time Elapsed: 696.894 s. Mean Reward: -2.469. Std of Reward: 0.503. Training.
[INFO] SpaceCleaner. Step: 140000. Time Elapsed: 748.000 s. Mean Reward: -2.247. Std of Reward: 1.293. Training.
[INFO] SpaceCleaner. Step: 150000. Time Elapsed: 797.972 s. Mean Reward: -2.389. Std of Reward: 0.465. Training.
[INFO] SpaceCleaner. Step: 160000. Time Elapsed: 851.370 s. Mean Reward: -2.309. Std of Reward: 0.558. Training.
[INFO] SpaceCleaner. Step: 170000. Time Elapsed: 902.259 s. Mean Reward: -2.489. Std of Reward: 0.532. Training.
[INFO] SpaceCleaner. Step: 180000. Time Elapsed: 953.464 s. Mean Reward: -2.306. Std of Reward: 1.367. Training.
[INFO] SpaceCleaner. Step: 190000. Time Elapsed: 1006.534 s. Mean Reward: -2.523. Std of Reward: 0.474. Training.
[INFO] SpaceCleaner. Step: 200000. Time Elapsed: 1056.764 s. Mean Reward: -2.204. Std of Reward: 0.696. Training.
[INFO] SpaceCleaner. Step: 210000. Time Elapsed: 1107.900 s. Mean Reward: -1.983. Std of Reward: 1.845. Training.
[INFO] SpaceCleaner. Step: 220000. Time Elapsed: 1160.698 s. Mean Reward: -2.412. Std of Reward: 1.260. Training.
[INFO] SpaceCleaner. Step: 230000. Time Elapsed: 1212.485 s. Mean Reward: -2.326. Std of Reward: 0.641. Training.
[INFO] SpaceCleaner. Step: 240000. Time Elapsed: 1263.459 s. Mean Reward: -1.951. Std of Reward: 2.114. Training.
[INFO] SpaceCleaner. Step: 250000. Time Elapsed: 1316.051 s. Mean Reward: -2.192. Std of Reward: 1.842. Training.
E:\Storage\Coding\Unity\Space Cleaning Sim2\.venv\lib\site-packages\torch\onnx\symbolic_opset9.py:4662: UserWarning: Exporting a model to ONNX with a batch_size other than 1, with a variable length with LSTM can cause an error when running the ONNX model with a different batch size. Make sure to save the model with a batch size of 1, or define the initial states (h0/c0) as inputs of the model. 
  warnings.warn(
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-249904.onnx
[INFO] SpaceCleaner. Step: 260000. Time Elapsed: 1367.053 s. Mean Reward: -2.759. Std of Reward: 1.226. Training.
[INFO] SpaceCleaner. Step: 270000. Time Elapsed: 1418.163 s. Mean Reward: -2.696. Std of Reward: 0.786. Training.
[INFO] SpaceCleaner. Step: 280000. Time Elapsed: 1471.354 s. Mean Reward: -2.913. Std of Reward: 2.381. Training.
[INFO] SpaceCleaner. Step: 290000. Time Elapsed: 1522.064 s. Mean Reward: -2.492. Std of Reward: 2.170. Training.
[INFO] SpaceCleaner. Step: 300000. Time Elapsed: 1573.065 s. Mean Reward: -2.715. Std of Reward: 0.787. Training.
[INFO] SpaceCleaner. Step: 310000. Time Elapsed: 1625.170 s. Mean Reward: -2.493. Std of Reward: 2.422. Training.
[INFO] SpaceCleaner. Step: 320000. Time Elapsed: 1676.765 s. Mean Reward: -2.705. Std of Reward: 2.657. Training.
[INFO] SpaceCleaner. Step: 330000. Time Elapsed: 1727.716 s. Mean Reward: -1.409. Std of Reward: 3.740. Training.
[INFO] SpaceCleaner. Step: 340000. Time Elapsed: 1780.106 s. Mean Reward: -2.309. Std of Reward: 0.826. Training.
[INFO] SpaceCleaner. Step: 350000. Time Elapsed: 1831.622 s. Mean Reward: -2.650. Std of Reward: 1.040. Training.
[INFO] SpaceCleaner. Step: 360000. Time Elapsed: 1883.646 s. Mean Reward: -1.897. Std of Reward: 2.922. Training.
[INFO] SpaceCleaner. Step: 370000. Time Elapsed: 1935.578 s. Mean Reward: -1.521. Std of Reward: 3.022. Training.
[INFO] SpaceCleaner. Step: 380000. Time Elapsed: 1987.291 s. Mean Reward: -1.468. Std of Reward: 3.816. Training.
[INFO] SpaceCleaner. Step: 390000. Time Elapsed: 2038.349 s. Mean Reward: 0.105. Std of Reward: 4.842. Training.
[INFO] SpaceCleaner. Step: 400000. Time Elapsed: 2090.663 s. Mean Reward: -2.814. Std of Reward: 3.676. Training.
[INFO] SpaceCleaner. Step: 410000. Time Elapsed: 2141.993 s. Mean Reward: -2.369. Std of Reward: 2.732. Training.
[INFO] SpaceCleaner. Step: 420000. Time Elapsed: 2192.402 s. Mean Reward: -0.110. Std of Reward: 4.261. Training.
[INFO] SpaceCleaner. Step: 430000. Time Elapsed: 2245.479 s. Mean Reward: -2.487. Std of Reward: 3.371. Training.
[INFO] SpaceCleaner. Step: 440000. Time Elapsed: 2296.635 s. Mean Reward: -1.806. Std of Reward: 6.450. Training.
[INFO] SpaceCleaner. Step: 450000. Time Elapsed: 2347.347 s. Mean Reward: -1.224. Std of Reward: 4.113. Training.
[INFO] SpaceCleaner. Step: 460000. Time Elapsed: 2400.378 s. Mean Reward: 0.540. Std of Reward: 3.837. Training.
[INFO] SpaceCleaner. Step: 470000. Time Elapsed: 2450.285 s. Mean Reward: -1.717. Std of Reward: 4.477. Training.
[INFO] SpaceCleaner. Step: 480000. Time Elapsed: 2501.438 s. Mean Reward: -1.715. Std of Reward: 3.430. Training.
[INFO] SpaceCleaner. Step: 490000. Time Elapsed: 2555.382 s. Mean Reward: -1.555. Std of Reward: 4.869. Training.
[INFO] SpaceCleaner. Step: 500000. Time Elapsed: 2606.168 s. Mean Reward: -4.334. Std of Reward: 3.811. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-499928.onnx
[INFO] SpaceCleaner. Step: 510000. Time Elapsed: 2656.676 s. Mean Reward: -1.606. Std of Reward: 2.341. Training.
[INFO] SpaceCleaner. Step: 520000. Time Elapsed: 2707.169 s. Mean Reward: -3.013. Std of Reward: 2.702. Training.
[INFO] SpaceCleaner. Step: 530000. Time Elapsed: 2760.972 s. Mean Reward: -4.185. Std of Reward: 4.701. Training.
[INFO] SpaceCleaner. Step: 540000. Time Elapsed: 2811.049 s. Mean Reward: -1.730. Std of Reward: 4.791. Training.
[INFO] SpaceCleaner. Step: 550000. Time Elapsed: 2862.458 s. Mean Reward: -2.181. Std of Reward: 4.948. Training.
[INFO] SpaceCleaner. Step: 560000. Time Elapsed: 2914.918 s. Mean Reward: -1.515. Std of Reward: 4.296. Training.
[INFO] SpaceCleaner. Step: 570000. Time Elapsed: 2965.755 s. Mean Reward: -3.590. Std of Reward: 3.423. Training.
[INFO] SpaceCleaner. Step: 580000. Time Elapsed: 3017.468 s. Mean Reward: -1.424. Std of Reward: 5.034. Training.
[INFO] SpaceCleaner. Step: 590000. Time Elapsed: 3069.985 s. Mean Reward: -0.476. Std of Reward: 4.165. Training.
[INFO] SpaceCleaner. Step: 600000. Time Elapsed: 3121.451 s. Mean Reward: -2.801. Std of Reward: 2.706. Training.
[INFO] SpaceCleaner. Step: 610000. Time Elapsed: 3172.448 s. Mean Reward: -2.027. Std of Reward: 3.882. Training.
[INFO] SpaceCleaner. Step: 620000. Time Elapsed: 3225.448 s. Mean Reward: -2.908. Std of Reward: 4.974. Training.
[INFO] SpaceCleaner. Step: 630000. Time Elapsed: 3276.508 s. Mean Reward: -3.782. Std of Reward: 4.755. Training.
[INFO] SpaceCleaner. Step: 640000. Time Elapsed: 3327.512 s. Mean Reward: -4.668. Std of Reward: 4.190. Training.
[INFO] SpaceCleaner. Step: 650000. Time Elapsed: 3381.004 s. Mean Reward: 2.416. Std of Reward: 4.975. Training.
[INFO] SpaceCleaner. Step: 660000. Time Elapsed: 3430.906 s. Mean Reward: -0.486. Std of Reward: 5.975. Training.
[INFO] SpaceCleaner. Step: 670000. Time Elapsed: 3481.898 s. Mean Reward: -2.434. Std of Reward: 5.653. Training.
[INFO] SpaceCleaner. Step: 680000. Time Elapsed: 3532.821 s. Mean Reward: -2.355. Std of Reward: 5.599. Training.
[INFO] SpaceCleaner. Step: 690000. Time Elapsed: 3587.129 s. Mean Reward: -1.427. Std of Reward: 3.900. Training.
[INFO] SpaceCleaner. Step: 700000. Time Elapsed: 3639.025 s. Mean Reward: -2.462. Std of Reward: 2.438. Training.
[INFO] SpaceCleaner. Step: 710000. Time Elapsed: 3690.294 s. Mean Reward: -1.500. Std of Reward: 4.645. Training.
[INFO] SpaceCleaner. Step: 720000. Time Elapsed: 3742.890 s. Mean Reward: -1.191. Std of Reward: 3.968. Training.
[INFO] SpaceCleaner. Step: 730000. Time Elapsed: 3794.109 s. Mean Reward: -2.218. Std of Reward: 4.570. Training.
[INFO] SpaceCleaner. Step: 740000. Time Elapsed: 3844.096 s. Mean Reward: -3.364. Std of Reward: 2.651. Training.
[INFO] SpaceCleaner. Step: 750000. Time Elapsed: 3897.952 s. Mean Reward: -0.829. Std of Reward: 3.825. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-749944.onnx
[INFO] SpaceCleaner. Step: 760000. Time Elapsed: 3949.424 s. Mean Reward: -1.467. Std of Reward: 4.837. Training.
[INFO] SpaceCleaner. Step: 770000. Time Elapsed: 3999.636 s. Mean Reward: 0.126. Std of Reward: 5.157. Training.
[INFO] SpaceCleaner. Step: 780000. Time Elapsed: 4052.078 s. Mean Reward: -0.133. Std of Reward: 4.924. Training.
[INFO] SpaceCleaner. Step: 790000. Time Elapsed: 4103.658 s. Mean Reward: 0.117. Std of Reward: 4.914. Training.
[INFO] SpaceCleaner. Step: 800000. Time Elapsed: 4155.074 s. Mean Reward: -1.107. Std of Reward: 3.858. Training.
[INFO] SpaceCleaner. Step: 810000. Time Elapsed: 4207.670 s. Mean Reward: -1.728. Std of Reward: 5.117. Training.
[INFO] SpaceCleaner. Step: 820000. Time Elapsed: 4258.335 s. Mean Reward: -1.086. Std of Reward: 3.582. Training.
[INFO] SpaceCleaner. Step: 830000. Time Elapsed: 4309.238 s. Mean Reward: -0.408. Std of Reward: 4.189. Training.
[INFO] SpaceCleaner. Step: 840000. Time Elapsed: 4363.057 s. Mean Reward: -0.239. Std of Reward: 4.973. Training.
[INFO] SpaceCleaner. Step: 850000. Time Elapsed: 4413.932 s. Mean Reward: 0.747. Std of Reward: 4.697. Training.
[INFO] SpaceCleaner. Step: 860000. Time Elapsed: 4464.238 s. Mean Reward: 0.318. Std of Reward: 5.521. Training.
[INFO] SpaceCleaner. Step: 870000. Time Elapsed: 4515.365 s. Mean Reward: -0.138. Std of Reward: 4.576. Training.
[INFO] SpaceCleaner. Step: 880000. Time Elapsed: 4568.078 s. Mean Reward: -0.145. Std of Reward: 4.948. Training.
[INFO] SpaceCleaner. Step: 890000. Time Elapsed: 4619.699 s. Mean Reward: -1.457. Std of Reward: 3.932. Training.
[INFO] SpaceCleaner. Step: 900000. Time Elapsed: 4670.810 s. Mean Reward: -0.663. Std of Reward: 3.988. Training.
[INFO] SpaceCleaner. Step: 910000. Time Elapsed: 4722.995 s. Mean Reward: -1.387. Std of Reward: 3.923. Training.
[INFO] SpaceCleaner. Step: 920000. Time Elapsed: 4773.882 s. Mean Reward: 0.299. Std of Reward: 5.068. Training.
[INFO] SpaceCleaner. Step: 930000. Time Elapsed: 4825.242 s. Mean Reward: -1.501. Std of Reward: 5.350. Training.
[INFO] SpaceCleaner. Step: 940000. Time Elapsed: 4877.919 s. Mean Reward: 0.831. Std of Reward: 5.000. Training.
[INFO] SpaceCleaner. Step: 950000. Time Elapsed: 4929.741 s. Mean Reward: 0.524. Std of Reward: 4.221. Training.
[INFO] SpaceCleaner. Step: 960000. Time Elapsed: 4980.385 s. Mean Reward: 1.709. Std of Reward: 5.653. Training.
[INFO] SpaceCleaner. Step: 970000. Time Elapsed: 5032.959 s. Mean Reward: -3.839. Std of Reward: 5.065. Training.
[INFO] SpaceCleaner. Step: 980000. Time Elapsed: 5084.146 s. Mean Reward: -0.056. Std of Reward: 4.558. Training.
[INFO] SpaceCleaner. Step: 990000. Time Elapsed: 5135.455 s. Mean Reward: -3.789. Std of Reward: 4.525. Training.
[INFO] SpaceCleaner. Step: 1000000. Time Elapsed: 5187.374 s. Mean Reward: -1.253. Std of Reward: 4.748. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-999913.onnx
[INFO] SpaceCleaner. Step: 1010000. Time Elapsed: 5239.103 s. Mean Reward: 0.920. Std of Reward: 3.769. Training.
[INFO] SpaceCleaner. Step: 1020000. Time Elapsed: 5289.393 s. Mean Reward: -0.385. Std of Reward: 4.119. Training.
[INFO] SpaceCleaner. Step: 1030000. Time Elapsed: 5342.327 s. Mean Reward: 0.503. Std of Reward: 3.739. Training.
[INFO] SpaceCleaner. Step: 1040000. Time Elapsed: 5392.772 s. Mean Reward: 0.486. Std of Reward: 4.792. Training.
[INFO] SpaceCleaner. Step: 1050000. Time Elapsed: 5444.808 s. Mean Reward: 0.042. Std of Reward: 3.452. Training.
[INFO] SpaceCleaner. Step: 1060000. Time Elapsed: 5495.646 s. Mean Reward: 0.445. Std of Reward: 5.395. Training.
[INFO] SpaceCleaner. Step: 1070000. Time Elapsed: 5548.727 s. Mean Reward: 0.926. Std of Reward: 4.382. Training.
[INFO] SpaceCleaner. Step: 1080000. Time Elapsed: 5599.460 s. Mean Reward: -1.593. Std of Reward: 4.666. Training.
[INFO] SpaceCleaner. Step: 1090000. Time Elapsed: 5649.714 s. Mean Reward: 0.264. Std of Reward: 3.490. Training.
[INFO] SpaceCleaner. Step: 1100000. Time Elapsed: 5703.719 s. Mean Reward: 2.115. Std of Reward: 4.278. Training.
[INFO] SpaceCleaner. Step: 1110000. Time Elapsed: 5753.792 s. Mean Reward: -0.389. Std of Reward: 4.555. Training.
[INFO] SpaceCleaner. Step: 1120000. Time Elapsed: 5805.404 s. Mean Reward: -0.490. Std of Reward: 6.841. Training.
[INFO] SpaceCleaner. Step: 1130000. Time Elapsed: 5857.647 s. Mean Reward: 2.097. Std of Reward: 3.811. Training.
[INFO] SpaceCleaner. Step: 1140000. Time Elapsed: 5908.393 s. Mean Reward: -0.650. Std of Reward: 3.742. Training.
[INFO] SpaceCleaner. Step: 1150000. Time Elapsed: 5959.946 s. Mean Reward: -0.155. Std of Reward: 3.869. Training.
[INFO] SpaceCleaner. Step: 1160000. Time Elapsed: 6012.830 s. Mean Reward: 1.847. Std of Reward: 5.027. Training.
[INFO] SpaceCleaner. Step: 1170000. Time Elapsed: 6063.046 s. Mean Reward: 0.168. Std of Reward: 3.864. Training.
[INFO] SpaceCleaner. Step: 1180000. Time Elapsed: 6113.601 s. Mean Reward: 0.458. Std of Reward: 3.609. Training.
[INFO] SpaceCleaner. Step: 1190000. Time Elapsed: 6167.599 s. Mean Reward: 2.044. Std of Reward: 5.729. Training.
[INFO] SpaceCleaner. Step: 1200000. Time Elapsed: 6217.535 s. Mean Reward: -1.031. Std of Reward: 3.137. Training.
[INFO] SpaceCleaner. Step: 1210000. Time Elapsed: 6269.511 s. Mean Reward: 1.405. Std of Reward: 5.223. Training.
[INFO] SpaceCleaner. Step: 1220000. Time Elapsed: 6321.439 s. Mean Reward: 0.092. Std of Reward: 4.296. Training.
[INFO] SpaceCleaner. Step: 1230000. Time Elapsed: 6373.328 s. Mean Reward: 0.955. Std of Reward: 3.867. Training.
[INFO] SpaceCleaner. Step: 1240000. Time Elapsed: 6424.154 s. Mean Reward: 1.400. Std of Reward: 4.976. Training.
[INFO] SpaceCleaner. Step: 1250000. Time Elapsed: 6478.052 s. Mean Reward: 2.375. Std of Reward: 4.799. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-1249889.onnx
[INFO] SpaceCleaner. Step: 1260000. Time Elapsed: 6528.463 s. Mean Reward: 2.390. Std of Reward: 4.475. Training.
[INFO] Parameter 'difficulty_level' is in lesson 'two-targets' and has value 'Float: value=0.11'.
[INFO] SpaceCleaner. Step: 1270000. Time Elapsed: 6579.615 s. Mean Reward: 2.237. Std of Reward: 3.855. Training.
[INFO] SpaceCleaner. Step: 1280000. Time Elapsed: 6630.618 s. Mean Reward: 1.191. Std of Reward: 4.360. Training.
[INFO] SpaceCleaner. Step: 1290000. Time Elapsed: 6682.688 s. Mean Reward: -1.741. Std of Reward: 3.894. Training.
[INFO] SpaceCleaner. Step: 1300000. Time Elapsed: 6733.542 s. Mean Reward: -1.940. Std of Reward: 3.273. Training.
[INFO] SpaceCleaner. Step: 1310000. Time Elapsed: 6783.176 s. Mean Reward: -6.161. Std of Reward: 3.052. Training.
[INFO] SpaceCleaner. Step: 1320000. Time Elapsed: 6836.714 s. Mean Reward: -4.459. Std of Reward: 2.915. Training.
[INFO] SpaceCleaner. Step: 1330000. Time Elapsed: 6886.987 s. Mean Reward: -2.546. Std of Reward: 2.891. Training.
[INFO] SpaceCleaner. Step: 1340000. Time Elapsed: 6938.016 s. Mean Reward: -2.161. Std of Reward: 3.783. Training.
[INFO] SpaceCleaner. Step: 1350000. Time Elapsed: 6992.136 s. Mean Reward: -0.969. Std of Reward: 5.567. Training.
[INFO] SpaceCleaner. Step: 1360000. Time Elapsed: 7043.027 s. Mean Reward: -1.599. Std of Reward: 4.089. Training.
[INFO] SpaceCleaner. Step: 1370000. Time Elapsed: 7093.939 s. Mean Reward: -0.692. Std of Reward: 3.888. Training.
[INFO] SpaceCleaner. Step: 1380000. Time Elapsed: 7147.084 s. Mean Reward: 0.375. Std of Reward: 3.772. Training.
[INFO] SpaceCleaner. Step: 1390000. Time Elapsed: 7197.796 s. Mean Reward: -1.966. Std of Reward: 2.912. Training.
[INFO] SpaceCleaner. Step: 1400000. Time Elapsed: 7249.138 s. Mean Reward: -3.007. Std of Reward: 2.900. Training.
[INFO] SpaceCleaner. Step: 1410000. Time Elapsed: 7302.651 s. Mean Reward: -2.440. Std of Reward: 3.745. Training.
[INFO] SpaceCleaner. Step: 1420000. Time Elapsed: 7353.696 s. Mean Reward: 0.009. Std of Reward: 5.739. Training.
[INFO] SpaceCleaner. Step: 1430000. Time Elapsed: 7404.467 s. Mean Reward: -1.277. Std of Reward: 6.290. Training.
[INFO] SpaceCleaner. Step: 1440000. Time Elapsed: 7455.892 s. Mean Reward: -4.572. Std of Reward: 4.447. Training.
[INFO] SpaceCleaner. Step: 1450000. Time Elapsed: 7508.755 s. Mean Reward: -1.344. Std of Reward: 4.563. Training.
[INFO] SpaceCleaner. Step: 1460000. Time Elapsed: 7561.161 s. Mean Reward: -2.594. Std of Reward: 2.305. Training.
[INFO] SpaceCleaner. Step: 1470000. Time Elapsed: 7612.629 s. Mean Reward: -0.317. Std of Reward: 4.715. Training.
[INFO] SpaceCleaner. Step: 1480000. Time Elapsed: 7665.311 s. Mean Reward: -0.776. Std of Reward: 4.113. Training.
[INFO] SpaceCleaner. Step: 1490000. Time Elapsed: 7716.330 s. Mean Reward: 0.384. Std of Reward: 4.436. Training.
[INFO] SpaceCleaner. Step: 1500000. Time Elapsed: 7768.470 s. Mean Reward: -0.514. Std of Reward: 5.218. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-1499966.onnx
[INFO] SpaceCleaner. Step: 1510000. Time Elapsed: 7821.431 s. Mean Reward: -1.051. Std of Reward: 4.059. Training.
[INFO] SpaceCleaner. Step: 1520000. Time Elapsed: 7872.052 s. Mean Reward: -3.088. Std of Reward: 4.778. Training.
[INFO] SpaceCleaner. Step: 1530000. Time Elapsed: 7923.816 s. Mean Reward: -2.447. Std of Reward: 2.259. Training.
[INFO] SpaceCleaner. Step: 1540000. Time Elapsed: 7976.312 s. Mean Reward: -3.955. Std of Reward: 2.949. Training.
[INFO] SpaceCleaner. Step: 1550000. Time Elapsed: 8028.028 s. Mean Reward: -0.208. Std of Reward: 3.084. Training.
[INFO] SpaceCleaner. Step: 1560000. Time Elapsed: 8078.636 s. Mean Reward: -0.738. Std of Reward: 3.598. Training.
[INFO] SpaceCleaner. Step: 1570000. Time Elapsed: 8132.467 s. Mean Reward: -4.627. Std of Reward: 3.368. Training.
[INFO] SpaceCleaner. Step: 1580000. Time Elapsed: 8183.425 s. Mean Reward: -3.320. Std of Reward: 2.769. Training.
[INFO] SpaceCleaner. Step: 1590000. Time Elapsed: 8234.634 s. Mean Reward: -2.377. Std of Reward: 5.566. Training.
[INFO] SpaceCleaner. Step: 1600000. Time Elapsed: 8285.797 s. Mean Reward: -1.678. Std of Reward: 3.638. Training.
[INFO] SpaceCleaner. Step: 1610000. Time Elapsed: 8339.736 s. Mean Reward: -1.807. Std of Reward: 4.753. Training.
[INFO] SpaceCleaner. Step: 1620000. Time Elapsed: 8390.389 s. Mean Reward: -1.255. Std of Reward: 1.817. Training.
[INFO] SpaceCleaner. Step: 1630000. Time Elapsed: 8441.211 s. Mean Reward: -3.410. Std of Reward: 3.048. Training.
[INFO] SpaceCleaner. Step: 1640000. Time Elapsed: 8494.221 s. Mean Reward: -1.381. Std of Reward: 5.013. Training.
[INFO] SpaceCleaner. Step: 1650000. Time Elapsed: 8545.967 s. Mean Reward: -0.247. Std of Reward: 3.638. Training.
[INFO] SpaceCleaner. Step: 1660000. Time Elapsed: 8596.112 s. Mean Reward: 0.625. Std of Reward: 4.345. Training.
[INFO] SpaceCleaner. Step: 1670000. Time Elapsed: 8649.788 s. Mean Reward: -0.986. Std of Reward: 5.348. Training.
[INFO] SpaceCleaner. Step: 1680000. Time Elapsed: 8700.952 s. Mean Reward: -1.782. Std of Reward: 5.164. Training.
[INFO] SpaceCleaner. Step: 1690000. Time Elapsed: 8751.663 s. Mean Reward: 0.340. Std of Reward: 3.907. Training.
[INFO] SpaceCleaner. Step: 1700000. Time Elapsed: 8804.703 s. Mean Reward: -2.871. Std of Reward: 2.733. Training.
[INFO] SpaceCleaner. Step: 1710000. Time Elapsed: 8856.536 s. Mean Reward: 0.075. Std of Reward: 3.652. Training.
[INFO] SpaceCleaner. Step: 1720000. Time Elapsed: 8908.242 s. Mean Reward: -1.225. Std of Reward: 4.989. Training.
[INFO] SpaceCleaner. Step: 1730000. Time Elapsed: 8961.118 s. Mean Reward: -3.069. Std of Reward: 2.722. Training.
[INFO] SpaceCleaner. Step: 1740000. Time Elapsed: 9012.021 s. Mean Reward: -2.878. Std of Reward: 2.595. Training.
[INFO] SpaceCleaner. Step: 1750000. Time Elapsed: 9062.259 s. Mean Reward: -2.360. Std of Reward: 4.243. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-1749779.onnx
[INFO] SpaceCleaner. Step: 1760000. Time Elapsed: 9113.942 s. Mean Reward: -2.129. Std of Reward: 3.634. Training.
[INFO] SpaceCleaner. Step: 1770000. Time Elapsed: 9167.912 s. Mean Reward: -1.557. Std of Reward: 3.893. Training.
[INFO] SpaceCleaner. Step: 1780000. Time Elapsed: 9217.965 s. Mean Reward: -0.408. Std of Reward: 5.718. Training.
[INFO] SpaceCleaner. Step: 1790000. Time Elapsed: 9270.038 s. Mean Reward: -0.749. Std of Reward: 6.915. Training.
[INFO] SpaceCleaner. Step: 1800000. Time Elapsed: 9323.425 s. Mean Reward: -1.331. Std of Reward: 6.025. Training.
[INFO] SpaceCleaner. Step: 1810000. Time Elapsed: 9373.992 s. Mean Reward: 0.484. Std of Reward: 3.038. Training.
[INFO] SpaceCleaner. Step: 1820000. Time Elapsed: 9425.215 s. Mean Reward: -1.174. Std of Reward: 2.233. Training.
[INFO] SpaceCleaner. Step: 1830000. Time Elapsed: 9479.038 s. Mean Reward: -1.006. Std of Reward: 5.957. Training.
[INFO] SpaceCleaner. Step: 1840000. Time Elapsed: 9529.677 s. Mean Reward: 0.275. Std of Reward: 4.410. Training.
[INFO] SpaceCleaner. Step: 1850000. Time Elapsed: 9581.195 s. Mean Reward: 0.119. Std of Reward: 2.630. Training.
[INFO] SpaceCleaner. Step: 1860000. Time Elapsed: 9634.414 s. Mean Reward: 0.182. Std of Reward: 4.505. Training.
[INFO] SpaceCleaner. Step: 1870000. Time Elapsed: 9685.274 s. Mean Reward: -0.033. Std of Reward: 4.935. Training.
[INFO] SpaceCleaner. Step: 1880000. Time Elapsed: 9736.733 s. Mean Reward: -0.757. Std of Reward: 6.837. Training.
[INFO] SpaceCleaner. Step: 1890000. Time Elapsed: 9790.815 s. Mean Reward: 0.826. Std of Reward: 4.557. Training.
[INFO] SpaceCleaner. Step: 1900000. Time Elapsed: 9841.258 s. Mean Reward: -0.169. Std of Reward: 6.857. Training.
[INFO] SpaceCleaner. Step: 1910000. Time Elapsed: 9892.512 s. Mean Reward: -0.604. Std of Reward: 8.617. Training.
[INFO] SpaceCleaner. Step: 1920000. Time Elapsed: 9943.381 s. Mean Reward: -0.657. Std of Reward: 6.391. Training.
[INFO] SpaceCleaner. Step: 1930000. Time Elapsed: 9996.648 s. Mean Reward: -1.145. Std of Reward: 4.087. Training.
[INFO] SpaceCleaner. Step: 1940000. Time Elapsed: 10048.299 s. Mean Reward: 0.382. Std of Reward: 7.258. Training.
[INFO] SpaceCleaner. Step: 1950000. Time Elapsed: 10099.740 s. Mean Reward: -2.742. Std of Reward: 4.984. Training.
[INFO] SpaceCleaner. Step: 1960000. Time Elapsed: 10152.170 s. Mean Reward: -0.827. Std of Reward: 2.983. Training.
[INFO] SpaceCleaner. Step: 1970000. Time Elapsed: 10203.812 s. Mean Reward: 0.336. Std of Reward: 4.217. Training.
[INFO] SpaceCleaner. Step: 1980000. Time Elapsed: 10255.350 s. Mean Reward: 0.237. Std of Reward: 4.621. Training.
[INFO] SpaceCleaner. Step: 1990000. Time Elapsed: 10308.985 s. Mean Reward: -0.919. Std of Reward: 6.818. Training.
[INFO] SpaceCleaner. Step: 2000000. Time Elapsed: 10359.714 s. Mean Reward: -1.696. Std of Reward: 4.023. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-1999850.onnx
[INFO] SpaceCleaner. Step: 2010000. Time Elapsed: 10434.281 s. Mean Reward: -1.513. Std of Reward: 2.197. Training.
[INFO] SpaceCleaner. Step: 2020000. Time Elapsed: 10509.985 s. Mean Reward: -2.161. Std of Reward: 6.015. Training.
[INFO] SpaceCleaner. Step: 2030000. Time Elapsed: 10583.576 s. Mean Reward: -0.203. Std of Reward: 3.289. Training.
[INFO] SpaceCleaner. Step: 2040000. Time Elapsed: 10656.288 s. Mean Reward: -1.069. Std of Reward: 3.876. Training.
[INFO] SpaceCleaner. Step: 2050000. Time Elapsed: 10733.842 s. Mean Reward: -0.030. Std of Reward: 5.069. Training.
[INFO] SpaceCleaner. Step: 2060000. Time Elapsed: 10807.079 s. Mean Reward: 0.509. Std of Reward: 4.926. Training.
[INFO] SpaceCleaner. Step: 2070000. Time Elapsed: 10879.635 s. Mean Reward: -0.404. Std of Reward: 4.004. Training.
[INFO] SpaceCleaner. Step: 2080000. Time Elapsed: 10952.549 s. Mean Reward: 0.811. Std of Reward: 5.056. Training.
[INFO] SpaceCleaner. Step: 2090000. Time Elapsed: 11029.621 s. Mean Reward: 2.444. Std of Reward: 4.937. Training.
[INFO] SpaceCleaner. Step: 2100000. Time Elapsed: 11103.369 s. Mean Reward: 0.637. Std of Reward: 2.993. Training.
[INFO] SpaceCleaner. Step: 2110000. Time Elapsed: 11177.165 s. Mean Reward: -0.194. Std of Reward: 4.680. Training.
[INFO] SpaceCleaner. Step: 2120000. Time Elapsed: 11253.466 s. Mean Reward: 1.123. Std of Reward: 4.551. Training.
[INFO] SpaceCleaner. Step: 2130000. Time Elapsed: 11324.870 s. Mean Reward: -0.806. Std of Reward: 5.150. Training.
[INFO] SpaceCleaner. Step: 2140000. Time Elapsed: 11398.935 s. Mean Reward: 0.974. Std of Reward: 5.063. Training.
[INFO] SpaceCleaner. Step: 2150000. Time Elapsed: 11475.559 s. Mean Reward: 0.281. Std of Reward: 5.569. Training.
[INFO] SpaceCleaner. Step: 2160000. Time Elapsed: 11547.101 s. Mean Reward: -0.915. Std of Reward: 5.843. Training.
[INFO] SpaceCleaner. Step: 2170000. Time Elapsed: 11621.336 s. Mean Reward: 0.127. Std of Reward: 5.298. Training.
[INFO] SpaceCleaner. Step: 2180000. Time Elapsed: 11695.987 s. Mean Reward: 1.637. Std of Reward: 5.149. Training.
[INFO] SpaceCleaner. Step: 2190000. Time Elapsed: 11770.009 s. Mean Reward: -0.772. Std of Reward: 5.831. Training.
[INFO] SpaceCleaner. Step: 2200000. Time Elapsed: 11842.646 s. Mean Reward: 0.612. Std of Reward: 3.640. Training.
[INFO] SpaceCleaner. Step: 2210000. Time Elapsed: 11919.794 s. Mean Reward: 2.107. Std of Reward: 4.925. Training.
[INFO] SpaceCleaner. Step: 2220000. Time Elapsed: 11992.318 s. Mean Reward: 1.250. Std of Reward: 4.362. Training.
[INFO] SpaceCleaner. Step: 2230000. Time Elapsed: 12065.983 s. Mean Reward: -0.543. Std of Reward: 4.641. Training.
[INFO] SpaceCleaner. Step: 2240000. Time Elapsed: 12138.784 s. Mean Reward: 0.850. Std of Reward: 3.535. Training.
[INFO] SpaceCleaner. Step: 2250000. Time Elapsed: 12214.647 s. Mean Reward: -0.676. Std of Reward: 3.639. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-2249777.onnx
[INFO] SpaceCleaner. Step: 2260000. Time Elapsed: 12289.753 s. Mean Reward: 2.089. Std of Reward: 5.031. Training.
[INFO] SpaceCleaner. Step: 2270000. Time Elapsed: 12362.418 s. Mean Reward: 2.747. Std of Reward: 5.829. Training.
[INFO] SpaceCleaner. Step: 2280000. Time Elapsed: 12437.207 s. Mean Reward: -0.245. Std of Reward: 4.351. Training.
[INFO] SpaceCleaner. Step: 2290000. Time Elapsed: 12510.189 s. Mean Reward: 1.681. Std of Reward: 3.115. Training.
[INFO] SpaceCleaner. Step: 2300000. Time Elapsed: 12583.647 s. Mean Reward: -1.099. Std of Reward: 4.451. Training.
[INFO] SpaceCleaner. Step: 2310000. Time Elapsed: 12659.216 s. Mean Reward: 0.212. Std of Reward: 3.616. Training.
[INFO] SpaceCleaner. Step: 2320000. Time Elapsed: 12732.375 s. Mean Reward: 0.651. Std of Reward: 6.695. Training.
[INFO] SpaceCleaner. Step: 2330000. Time Elapsed: 12805.884 s. Mean Reward: -0.593. Std of Reward: 4.093. Training.
[INFO] SpaceCleaner. Step: 2340000. Time Elapsed: 12882.684 s. Mean Reward: 1.420. Std of Reward: 3.955. Training.
[INFO] SpaceCleaner. Step: 2350000. Time Elapsed: 12955.269 s. Mean Reward: 1.214. Std of Reward: 5.833. Training.
[INFO] SpaceCleaner. Step: 2360000. Time Elapsed: 13028.975 s. Mean Reward: 3.537. Std of Reward: 6.320. Training.
[INFO] SpaceCleaner. Step: 2370000. Time Elapsed: 13105.636 s. Mean Reward: 1.511. Std of Reward: 6.505. Training.
[INFO] SpaceCleaner. Step: 2380000. Time Elapsed: 13179.145 s. Mean Reward: 0.602. Std of Reward: 3.205. Training.
[INFO] SpaceCleaner. Step: 2390000. Time Elapsed: 13250.994 s. Mean Reward: -0.028. Std of Reward: 3.463. Training.
[INFO] SpaceCleaner. Step: 2400000. Time Elapsed: 13330.665 s. Mean Reward: -1.822. Std of Reward: 3.404. Training.
[INFO] SpaceCleaner. Step: 2410000. Time Elapsed: 13400.531 s. Mean Reward: 2.159. Std of Reward: 6.625. Training.
[INFO] SpaceCleaner. Step: 2420000. Time Elapsed: 13472.611 s. Mean Reward: 1.750. Std of Reward: 4.818. Training.
[INFO] SpaceCleaner. Step: 2430000. Time Elapsed: 13546.540 s. Mean Reward: 1.652. Std of Reward: 4.537. Training.
[INFO] SpaceCleaner. Step: 2440000. Time Elapsed: 13621.858 s. Mean Reward: -0.188. Std of Reward: 5.648. Training.
[INFO] SpaceCleaner. Step: 2450000. Time Elapsed: 13696.435 s. Mean Reward: 1.268. Std of Reward: 3.260. Training.
[INFO] SpaceCleaner. Step: 2460000. Time Elapsed: 13768.653 s. Mean Reward: 1.450. Std of Reward: 4.728. Training.
[INFO] SpaceCleaner. Step: 2470000. Time Elapsed: 13844.682 s. Mean Reward: 3.428. Std of Reward: 6.419. Training.
[INFO] SpaceCleaner. Step: 2480000. Time Elapsed: 13917.696 s. Mean Reward: 2.240. Std of Reward: 6.021. Training.
[INFO] SpaceCleaner. Step: 2490000. Time Elapsed: 13992.085 s. Mean Reward: 2.871. Std of Reward: 6.975. Training.
[INFO] SpaceCleaner. Step: 2500000. Time Elapsed: 14069.124 s. Mean Reward: 0.818. Std of Reward: 4.835. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-2499953.onnx
[INFO] SpaceCleaner. Step: 2510000. Time Elapsed: 14142.159 s. Mean Reward: 0.319. Std of Reward: 3.683. Training.
[INFO] SpaceCleaner. Step: 2520000. Time Elapsed: 14215.279 s. Mean Reward: 0.580. Std of Reward: 4.518. Training.
[INFO] SpaceCleaner. Step: 2530000. Time Elapsed: 14292.304 s. Mean Reward: 0.804. Std of Reward: 4.426. Training.
[INFO] SpaceCleaner. Step: 2540000. Time Elapsed: 14364.355 s. Mean Reward: 2.554. Std of Reward: 5.347. Training.
[INFO] SpaceCleaner. Step: 2550000. Time Elapsed: 14437.404 s. Mean Reward: -2.528. Std of Reward: 7.263. Training.
[INFO] SpaceCleaner. Step: 2560000. Time Elapsed: 14516.809 s. Mean Reward: 2.065. Std of Reward: 5.502. Training.
[INFO] SpaceCleaner. Step: 2570000. Time Elapsed: 14588.269 s. Mean Reward: 0.601. Std of Reward: 5.890. Training.
[INFO] SpaceCleaner. Step: 2580000. Time Elapsed: 14662.942 s. Mean Reward: 0.707. Std of Reward: 4.147. Training.
[INFO] SpaceCleaner. Step: 2590000. Time Elapsed: 14735.317 s. Mean Reward: 2.667. Std of Reward: 5.260. Training.
[INFO] SpaceCleaner. Step: 2600000. Time Elapsed: 14811.940 s. Mean Reward: 1.963. Std of Reward: 4.827. Training.
[INFO] SpaceCleaner. Step: 2610000. Time Elapsed: 14885.496 s. Mean Reward: -0.299. Std of Reward: 3.912. Training.
[INFO] SpaceCleaner. Step: 2620000. Time Elapsed: 14960.122 s. Mean Reward: 2.068. Std of Reward: 4.669. Training.
[INFO] SpaceCleaner. Step: 2630000. Time Elapsed: 15034.453 s. Mean Reward: 1.879. Std of Reward: 5.828. Training.
[INFO] SpaceCleaner. Step: 2640000. Time Elapsed: 15107.872 s. Mean Reward: 1.974. Std of Reward: 3.915. Training.
[INFO] SpaceCleaner. Step: 2650000. Time Elapsed: 15180.973 s. Mean Reward: 1.219. Std of Reward: 4.140. Training.
[INFO] SpaceCleaner. Step: 2660000. Time Elapsed: 15257.086 s. Mean Reward: 2.397. Std of Reward: 4.899. Training.
[INFO] SpaceCleaner. Step: 2670000. Time Elapsed: 15330.471 s. Mean Reward: -0.903. Std of Reward: 2.853. Training.
[INFO] SpaceCleaner. Step: 2680000. Time Elapsed: 15404.018 s. Mean Reward: 1.134. Std of Reward: 4.299. Training.
[INFO] SpaceCleaner. Step: 2690000. Time Elapsed: 15481.761 s. Mean Reward: 0.249. Std of Reward: 4.434. Training.
[INFO] SpaceCleaner. Step: 2700000. Time Elapsed: 15555.291 s. Mean Reward: 0.464. Std of Reward: 4.801. Training.
[INFO] SpaceCleaner. Step: 2710000. Time Elapsed: 15628.072 s. Mean Reward: 2.036. Std of Reward: 4.470. Training.
[INFO] SpaceCleaner. Step: 2720000. Time Elapsed: 15703.779 s. Mean Reward: 2.266. Std of Reward: 5.520. Training.
[INFO] SpaceCleaner. Step: 2730000. Time Elapsed: 15776.972 s. Mean Reward: 1.483. Std of Reward: 6.014. Training.
[INFO] SpaceCleaner. Step: 2740000. Time Elapsed: 15850.491 s. Mean Reward: 1.261. Std of Reward: 5.669. Training.
[INFO] SpaceCleaner. Step: 2750000. Time Elapsed: 15933.077 s. Mean Reward: 3.479. Std of Reward: 4.409. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-2749990.onnx
[INFO] SpaceCleaner. Step: 2760000. Time Elapsed: 16001.554 s. Mean Reward: 2.554. Std of Reward: 5.035. Training.
[INFO] SpaceCleaner. Step: 2770000. Time Elapsed: 16073.950 s. Mean Reward: 1.894. Std of Reward: 4.713. Training.
[INFO] SpaceCleaner. Step: 2780000. Time Elapsed: 16148.321 s. Mean Reward: 2.556. Std of Reward: 5.315. Training.
[INFO] SpaceCleaner. Step: 2790000. Time Elapsed: 16224.239 s. Mean Reward: 4.745. Std of Reward: 5.326. Training.
[INFO] SpaceCleaner. Step: 2800000. Time Elapsed: 16297.962 s. Mean Reward: 1.417. Std of Reward: 4.916. Training.
[INFO] Parameter 'difficulty_level' is in lesson 'four-targets' and has value 'Float: value=0.33'.
[INFO] SpaceCleaner. Step: 2810000. Time Elapsed: 16370.206 s. Mean Reward: 2.170. Std of Reward: 3.470. Training.
[INFO] SpaceCleaner. Step: 2820000. Time Elapsed: 16444.945 s. Mean Reward: 2.907. Std of Reward: 5.978. Training.
[INFO] SpaceCleaner. Step: 2830000. Time Elapsed: 16517.859 s. Mean Reward: 0.336. Std of Reward: 5.862. Training.
[INFO] SpaceCleaner. Step: 2840000. Time Elapsed: 16590.124 s. Mean Reward: -0.062. Std of Reward: 7.116. Training.
[INFO] SpaceCleaner. Step: 2850000. Time Elapsed: 16665.613 s. Mean Reward: -0.145. Std of Reward: 2.183. Training.
[INFO] SpaceCleaner. Step: 2860000. Time Elapsed: 16739.049 s. Mean Reward: 2.879. Std of Reward: 4.673. Training.
[INFO] SpaceCleaner. Step: 2870000. Time Elapsed: 16811.771 s. Mean Reward: -1.175. Std of Reward: 1.975. Training.
[INFO] SpaceCleaner. Step: 2880000. Time Elapsed: 16887.503 s. Mean Reward: 3.137. Std of Reward: 5.601. Training.
[INFO] SpaceCleaner. Step: 2890000. Time Elapsed: 16961.760 s. Mean Reward: -1.507. Std of Reward: 2.953. Training.
[INFO] SpaceCleaner. Step: 2900000. Time Elapsed: 17033.946 s. Mean Reward: -0.213. Std of Reward: 5.280. Training.
[INFO] SpaceCleaner. Step: 2910000. Time Elapsed: 17115.298 s. Mean Reward: 2.004. Std of Reward: 5.467. Training.
[INFO] SpaceCleaner. Step: 2920000. Time Elapsed: 17185.856 s. Mean Reward: 0.057. Std of Reward: 4.444. Training.
[INFO] SpaceCleaner. Step: 2930000. Time Elapsed: 17257.750 s. Mean Reward: 0.135. Std of Reward: 4.154. Training.
[INFO] SpaceCleaner. Step: 2940000. Time Elapsed: 17332.841 s. Mean Reward: -0.955. Std of Reward: 1.837. Training.
[INFO] SpaceCleaner. Step: 2950000. Time Elapsed: 17408.136 s. Mean Reward: -1.016. Std of Reward: 2.335. Training.
[INFO] SpaceCleaner. Step: 2960000. Time Elapsed: 17482.138 s. Mean Reward: -0.720. Std of Reward: 1.766. Training.
[INFO] SpaceCleaner. Step: 2970000. Time Elapsed: 17555.845 s. Mean Reward: 0.583. Std of Reward: 3.294. Training.
[INFO] SpaceCleaner. Step: 2980000. Time Elapsed: 17631.433 s. Mean Reward: 1.134. Std of Reward: 5.011. Training.
[INFO] SpaceCleaner. Step: 2990000. Time Elapsed: 17705.684 s. Mean Reward: 0.195. Std of Reward: 4.222. Training.
[INFO] SpaceCleaner. Step: 3000000. Time Elapsed: 17779.768 s. Mean Reward: 0.098. Std of Reward: 3.370. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-2999775.onnx
[INFO] SpaceCleaner. Step: 3010000. Time Elapsed: 17855.851 s. Mean Reward: 1.028. Std of Reward: 5.061. Training.
[INFO] SpaceCleaner. Step: 3020000. Time Elapsed: 17930.883 s. Mean Reward: -0.241. Std of Reward: 3.464. Training.
[INFO] SpaceCleaner. Step: 3030000. Time Elapsed: 18006.445 s. Mean Reward: 1.977. Std of Reward: 4.133. Training.
[INFO] SpaceCleaner. Step: 3040000. Time Elapsed: 18084.195 s. Mean Reward: -0.997. Std of Reward: 1.648. Training.
[INFO] SpaceCleaner. Step: 3050000. Time Elapsed: 18158.053 s. Mean Reward: -0.023. Std of Reward: 3.670. Training.
[INFO] SpaceCleaner. Step: 3060000. Time Elapsed: 18231.093 s. Mean Reward: 0.419. Std of Reward: 6.130. Training.
[INFO] SpaceCleaner. Step: 3070000. Time Elapsed: 18312.204 s. Mean Reward: -1.134. Std of Reward: 4.312. Training.
[INFO] SpaceCleaner. Step: 3080000. Time Elapsed: 18381.109 s. Mean Reward: 2.328. Std of Reward: 5.297. Training.
[INFO] SpaceCleaner. Step: 3090000. Time Elapsed: 18455.827 s. Mean Reward: 0.423. Std of Reward: 2.721. Training.
[INFO] SpaceCleaner. Step: 3100000. Time Elapsed: 18528.290 s. Mean Reward: 0.619. Std of Reward: 5.316. Training.
[INFO] SpaceCleaner. Step: 3110000. Time Elapsed: 18603.940 s. Mean Reward: -0.106. Std of Reward: 4.166. Training.
[INFO] SpaceCleaner. Step: 3120000. Time Elapsed: 18678.424 s. Mean Reward: 1.347. Std of Reward: 5.011. Training.
[INFO] SpaceCleaner. Step: 3130000. Time Elapsed: 18752.794 s. Mean Reward: 3.415. Std of Reward: 6.376. Training.
[INFO] SpaceCleaner. Step: 3140000. Time Elapsed: 18828.411 s. Mean Reward: 1.157. Std of Reward: 3.121. Training.
[INFO] SpaceCleaner. Step: 3150000. Time Elapsed: 18902.416 s. Mean Reward: 1.199. Std of Reward: 4.135. Training.
[INFO] SpaceCleaner. Step: 3160000. Time Elapsed: 18976.111 s. Mean Reward: -0.115. Std of Reward: 3.435. Training.
[INFO] SpaceCleaner. Step: 3170000. Time Elapsed: 19053.474 s. Mean Reward: 1.713. Std of Reward: 2.310. Training.
[INFO] SpaceCleaner. Step: 3180000. Time Elapsed: 19126.357 s. Mean Reward: 1.281. Std of Reward: 5.328. Training.
[INFO] SpaceCleaner. Step: 3190000. Time Elapsed: 19199.526 s. Mean Reward: 2.156. Std of Reward: 4.996. Training.
[INFO] SpaceCleaner. Step: 3200000. Time Elapsed: 19277.506 s. Mean Reward: 1.373. Std of Reward: 4.910. Training.
[INFO] SpaceCleaner. Step: 3210000. Time Elapsed: 19350.719 s. Mean Reward: 2.518. Std of Reward: 5.946. Training.
[INFO] SpaceCleaner. Step: 3220000. Time Elapsed: 19422.877 s. Mean Reward: 0.749. Std of Reward: 4.509. Training.
[INFO] SpaceCleaner. Step: 3230000. Time Elapsed: 19500.465 s. Mean Reward: 2.087. Std of Reward: 5.923. Training.
[INFO] SpaceCleaner. Step: 3240000. Time Elapsed: 19572.382 s. Mean Reward: 0.509. Std of Reward: 6.479. Training.
[INFO] SpaceCleaner. Step: 3250000. Time Elapsed: 19647.285 s. Mean Reward: 2.147. Std of Reward: 6.564. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-3249933.onnx
[INFO] SpaceCleaner. Step: 3260000. Time Elapsed: 19721.878 s. Mean Reward: 2.755. Std of Reward: 5.787. Training.
[INFO] SpaceCleaner. Step: 3270000. Time Elapsed: 19797.118 s. Mean Reward: 2.643. Std of Reward: 6.668. Training.
[INFO] SpaceCleaner. Step: 3280000. Time Elapsed: 19870.547 s. Mean Reward: 0.412. Std of Reward: 4.318. Training.
[INFO] SpaceCleaner. Step: 3290000. Time Elapsed: 19945.313 s. Mean Reward: 1.295. Std of Reward: 3.386. Training.
[INFO] SpaceCleaner. Step: 3300000. Time Elapsed: 20020.965 s. Mean Reward: 0.084. Std of Reward: 4.331. Training.
[INFO] SpaceCleaner. Step: 3310000. Time Elapsed: 20095.392 s. Mean Reward: -0.159. Std of Reward: 3.463. Training.
[INFO] SpaceCleaner. Step: 3320000. Time Elapsed: 20171.085 s. Mean Reward: -0.159. Std of Reward: 3.415. Training.
[INFO] SpaceCleaner. Step: 3330000. Time Elapsed: 20249.202 s. Mean Reward: 2.283. Std of Reward: 4.824. Training.
[INFO] SpaceCleaner. Step: 3340000. Time Elapsed: 20324.352 s. Mean Reward: -0.170. Std of Reward: 1.660. Training.
[INFO] SpaceCleaner. Step: 3350000. Time Elapsed: 20397.338 s. Mean Reward: 3.868. Std of Reward: 5.771. Training.
[INFO] SpaceCleaner. Step: 3360000. Time Elapsed: 20474.505 s. Mean Reward: 2.912. Std of Reward: 6.125. Training.
[INFO] SpaceCleaner. Step: 3370000. Time Elapsed: 20547.896 s. Mean Reward: 3.141. Std of Reward: 5.668. Training.
[INFO] SpaceCleaner. Step: 3380000. Time Elapsed: 20621.584 s. Mean Reward: 2.289. Std of Reward: 6.274. Training.
[INFO] SpaceCleaner. Step: 3390000. Time Elapsed: 20698.822 s. Mean Reward: 1.584. Std of Reward: 5.553. Training.
[INFO] SpaceCleaner. Step: 3400000. Time Elapsed: 20771.504 s. Mean Reward: 1.929. Std of Reward: 1.779. Training.
[INFO] SpaceCleaner. Step: 3410000. Time Elapsed: 20844.672 s. Mean Reward: 2.408. Std of Reward: 4.766. Training.
[INFO] SpaceCleaner. Step: 3420000. Time Elapsed: 20920.191 s. Mean Reward: 2.419. Std of Reward: 7.198. Training.
[INFO] SpaceCleaner. Step: 3430000. Time Elapsed: 20995.884 s. Mean Reward: 2.418. Std of Reward: 6.005. Training.
[INFO] SpaceCleaner. Step: 3440000. Time Elapsed: 21069.677 s. Mean Reward: 1.862. Std of Reward: 5.786. Training.
[INFO] SpaceCleaner. Step: 3450000. Time Elapsed: 21143.364 s. Mean Reward: 0.032. Std of Reward: 3.660. Training.
[INFO] SpaceCleaner. Step: 3460000. Time Elapsed: 21218.655 s. Mean Reward: 2.985. Std of Reward: 3.921. Training.
[INFO] SpaceCleaner. Step: 3470000. Time Elapsed: 21293.190 s. Mean Reward: 0.424. Std of Reward: 3.292. Training.
[INFO] SpaceCleaner. Step: 3480000. Time Elapsed: 21366.546 s. Mean Reward: 0.116. Std of Reward: 2.060. Training.
[INFO] SpaceCleaner. Step: 3490000. Time Elapsed: 21443.841 s. Mean Reward: 0.392. Std of Reward: 3.017. Training.
[INFO] SpaceCleaner. Step: 3500000. Time Elapsed: 21517.467 s. Mean Reward: 4.331. Std of Reward: 7.800. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-3499938.onnx
[INFO] SpaceCleaner. Step: 3510000. Time Elapsed: 21590.997 s. Mean Reward: 2.054. Std of Reward: 5.180. Training.
[INFO] SpaceCleaner. Step: 3520000. Time Elapsed: 21667.936 s. Mean Reward: 4.465. Std of Reward: 5.728. Training.
[INFO] SpaceCleaner. Step: 3530000. Time Elapsed: 21741.822 s. Mean Reward: 2.182. Std of Reward: 5.155. Training.
[INFO] SpaceCleaner. Step: 3540000. Time Elapsed: 21815.893 s. Mean Reward: 2.847. Std of Reward: 5.582. Training.
[INFO] SpaceCleaner. Step: 3550000. Time Elapsed: 21893.839 s. Mean Reward: 1.357. Std of Reward: 4.547. Training.
[INFO] SpaceCleaner. Step: 3560000. Time Elapsed: 21965.840 s. Mean Reward: 3.900. Std of Reward: 5.508. Training.
[INFO] SpaceCleaner. Step: 3570000. Time Elapsed: 22040.133 s. Mean Reward: 2.057. Std of Reward: 3.923. Training.
[INFO] SpaceCleaner. Step: 3580000. Time Elapsed: 22117.894 s. Mean Reward: 1.643. Std of Reward: 3.508. Training.
[INFO] SpaceCleaner. Step: 3590000. Time Elapsed: 22191.186 s. Mean Reward: 4.319. Std of Reward: 6.632. Training.
[INFO] SpaceCleaner. Step: 3600000. Time Elapsed: 22265.026 s. Mean Reward: 0.505. Std of Reward: 2.866. Training.
[INFO] SpaceCleaner. Step: 3610000. Time Elapsed: 22338.195 s. Mean Reward: 0.346. Std of Reward: 3.729. Training.
[INFO] SpaceCleaner. Step: 3620000. Time Elapsed: 22414.446 s. Mean Reward: 3.126. Std of Reward: 6.147. Training.
[INFO] SpaceCleaner. Step: 3630000. Time Elapsed: 22488.222 s. Mean Reward: 3.465. Std of Reward: 5.227. Training.
[INFO] SpaceCleaner. Step: 3640000. Time Elapsed: 22563.185 s. Mean Reward: 5.373. Std of Reward: 6.729. Training.
[INFO] SpaceCleaner. Step: 3650000. Time Elapsed: 22639.102 s. Mean Reward: 0.294. Std of Reward: 2.163. Training.
[INFO] SpaceCleaner. Step: 3660000. Time Elapsed: 22713.785 s. Mean Reward: 0.396. Std of Reward: 3.387. Training.
[INFO] SpaceCleaner. Step: 3670000. Time Elapsed: 22786.583 s. Mean Reward: 5.177. Std of Reward: 5.078. Training.
[INFO] SpaceCleaner. Step: 3680000. Time Elapsed: 22863.631 s. Mean Reward: 3.232. Std of Reward: 6.124. Training.
[INFO] SpaceCleaner. Step: 3690000. Time Elapsed: 22936.606 s. Mean Reward: 5.805. Std of Reward: 7.409. Training.
[INFO] SpaceCleaner. Step: 3700000. Time Elapsed: 23010.109 s. Mean Reward: 1.256. Std of Reward: 3.253. Training.
[INFO] SpaceCleaner. Step: 3710000. Time Elapsed: 23087.559 s. Mean Reward: 0.863. Std of Reward: 3.426. Training.
[INFO] SpaceCleaner. Step: 3720000. Time Elapsed: 23160.146 s. Mean Reward: 2.799. Std of Reward: 5.225. Training.
[INFO] SpaceCleaner. Step: 3730000. Time Elapsed: 23233.705 s. Mean Reward: 1.437. Std of Reward: 2.015. Training.
[INFO] SpaceCleaner. Step: 3740000. Time Elapsed: 23309.802 s. Mean Reward: 3.230. Std of Reward: 5.351. Training.
[INFO] SpaceCleaner. Step: 3750000. Time Elapsed: 23384.852 s. Mean Reward: 0.763. Std of Reward: 4.119. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-3749990.onnx
[INFO] SpaceCleaner. Step: 3760000. Time Elapsed: 23458.304 s. Mean Reward: 3.641. Std of Reward: 4.184. Training.
[INFO] SpaceCleaner. Step: 3770000. Time Elapsed: 23535.503 s. Mean Reward: 3.957. Std of Reward: 6.370. Training.
[INFO] SpaceCleaner. Step: 3780000. Time Elapsed: 23607.504 s. Mean Reward: -0.357. Std of Reward: 2.326. Training.
[INFO] SpaceCleaner. Step: 3790000. Time Elapsed: 23681.227 s. Mean Reward: 4.501. Std of Reward: 6.866. Training.
[INFO] SpaceCleaner. Step: 3800000. Time Elapsed: 23756.562 s. Mean Reward: 1.320. Std of Reward: 3.957. Training.
[INFO] SpaceCleaner. Step: 3810000. Time Elapsed: 23832.304 s. Mean Reward: 2.786. Std of Reward: 5.193. Training.
[INFO] SpaceCleaner. Step: 3820000. Time Elapsed: 23905.818 s. Mean Reward: 4.315. Std of Reward: 6.194. Training.
[INFO] SpaceCleaner. Step: 3830000. Time Elapsed: 23978.843 s. Mean Reward: 1.089. Std of Reward: 4.515. Training.
[INFO] SpaceCleaner. Step: 3840000. Time Elapsed: 24055.281 s. Mean Reward: 2.163. Std of Reward: 4.794. Training.
[INFO] SpaceCleaner. Step: 3850000. Time Elapsed: 24129.587 s. Mean Reward: 1.509. Std of Reward: 4.514. Training.
[INFO] SpaceCleaner. Step: 3860000. Time Elapsed: 24203.362 s. Mean Reward: 2.698. Std of Reward: 5.041. Training.
[INFO] SpaceCleaner. Step: 3870000. Time Elapsed: 24278.560 s. Mean Reward: 3.590. Std of Reward: 5.822. Training.
[INFO] SpaceCleaner. Step: 3880000. Time Elapsed: 24352.528 s. Mean Reward: 5.430. Std of Reward: 7.006. Training.
[INFO] SpaceCleaner. Step: 3890000. Time Elapsed: 24426.550 s. Mean Reward: 4.117. Std of Reward: 5.008. Training.
[INFO] SpaceCleaner. Step: 3900000. Time Elapsed: 24503.613 s. Mean Reward: 4.561. Std of Reward: 6.823. Training.
[INFO] SpaceCleaner. Step: 3910000. Time Elapsed: 24576.120 s. Mean Reward: 5.280. Std of Reward: 6.305. Training.
[INFO] SpaceCleaner. Step: 3920000. Time Elapsed: 24651.533 s. Mean Reward: 0.174. Std of Reward: 4.072. Training.
[INFO] SpaceCleaner. Step: 3930000. Time Elapsed: 24726.816 s. Mean Reward: 1.364. Std of Reward: 3.493. Training.
[INFO] SpaceCleaner. Step: 3940000. Time Elapsed: 24799.968 s. Mean Reward: 1.118. Std of Reward: 5.095. Training.
[INFO] SpaceCleaner. Step: 3950000. Time Elapsed: 24874.029 s. Mean Reward: 3.217. Std of Reward: 6.395. Training.
[INFO] SpaceCleaner. Step: 3960000. Time Elapsed: 24950.581 s. Mean Reward: 4.460. Std of Reward: 7.919. Training.
[INFO] SpaceCleaner. Step: 3970000. Time Elapsed: 25024.155 s. Mean Reward: 1.909. Std of Reward: 5.457. Training.
[INFO] SpaceCleaner. Step: 3980000. Time Elapsed: 25097.656 s. Mean Reward: 2.094. Std of Reward: 4.652. Training.
[INFO] SpaceCleaner. Step: 3990000. Time Elapsed: 25171.995 s. Mean Reward: 3.661. Std of Reward: 6.417. Training.
[INFO] SpaceCleaner. Step: 4000000. Time Elapsed: 25248.218 s. Mean Reward: 3.849. Std of Reward: 5.006. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-3999766.onnx
[INFO] SpaceCleaner. Step: 4010000. Time Elapsed: 25324.116 s. Mean Reward: 1.530. Std of Reward: 3.324. Training.
[INFO] SpaceCleaner. Step: 4020000. Time Elapsed: 25398.425 s. Mean Reward: 1.500. Std of Reward: 3.014. Training.
[INFO] SpaceCleaner. Step: 4030000. Time Elapsed: 25475.604 s. Mean Reward: 1.434. Std of Reward: 5.138. Training.
[INFO] SpaceCleaner. Step: 4040000. Time Elapsed: 25548.903 s. Mean Reward: 4.083. Std of Reward: 6.139. Training.
[INFO] SpaceCleaner. Step: 4050000. Time Elapsed: 25624.022 s. Mean Reward: 2.133. Std of Reward: 4.928. Training.
[INFO] SpaceCleaner. Step: 4060000. Time Elapsed: 25700.310 s. Mean Reward: 4.615. Std of Reward: 4.823. Training.
[INFO] SpaceCleaner. Step: 4070000. Time Elapsed: 25774.236 s. Mean Reward: 7.332. Std of Reward: 6.758. Training.
[INFO] SpaceCleaner. Step: 4080000. Time Elapsed: 25847.483 s. Mean Reward: 3.409. Std of Reward: 5.799. Training.
[INFO] SpaceCleaner. Step: 4090000. Time Elapsed: 25925.094 s. Mean Reward: 3.352. Std of Reward: 6.497. Training.
[INFO] SpaceCleaner. Step: 4100000. Time Elapsed: 25998.809 s. Mean Reward: 4.391. Std of Reward: 5.516. Training.
[INFO] SpaceCleaner. Step: 4110000. Time Elapsed: 26073.043 s. Mean Reward: 4.671. Std of Reward: 7.419. Training.
[INFO] SpaceCleaner. Step: 4120000. Time Elapsed: 26148.403 s. Mean Reward: 1.729. Std of Reward: 2.812. Training.
[INFO] SpaceCleaner. Step: 4130000. Time Elapsed: 26222.841 s. Mean Reward: 3.015. Std of Reward: 5.578. Training.
[INFO] SpaceCleaner. Step: 4140000. Time Elapsed: 26296.558 s. Mean Reward: 4.487. Std of Reward: 7.114. Training.
[INFO] SpaceCleaner. Step: 4150000. Time Elapsed: 26376.044 s. Mean Reward: 2.588. Std of Reward: 5.293. Training.
[INFO] SpaceCleaner. Step: 4160000. Time Elapsed: 26448.050 s. Mean Reward: 4.424. Std of Reward: 7.183. Training.
[INFO] SpaceCleaner. Step: 4170000. Time Elapsed: 26521.412 s. Mean Reward: 8.805. Std of Reward: 6.610. Training.
[INFO] Parameter 'difficulty_level' is in lesson 'six-targets' and has value 'Float: value=0.56'.
[INFO] SpaceCleaner. Step: 4180000. Time Elapsed: 26594.494 s. Mean Reward: 2.771. Std of Reward: 6.054. Training.
[INFO] SpaceCleaner. Step: 4190000. Time Elapsed: 26669.720 s. Mean Reward: 1.214. Std of Reward: 4.455. Training.
[INFO] SpaceCleaner. Step: 4200000. Time Elapsed: 26744.517 s. Mean Reward: 3.118. Std of Reward: 5.473. Training.
[INFO] SpaceCleaner. Step: 4210000. Time Elapsed: 26816.360 s. Mean Reward: 0.535. Std of Reward: 5.674. Training.
[INFO] SpaceCleaner. Step: 4220000. Time Elapsed: 26892.214 s. Mean Reward: -0.464. Std of Reward: 2.618. Training.
[INFO] SpaceCleaner. Step: 4230000. Time Elapsed: 26964.589 s. Mean Reward: -0.056. Std of Reward: 5.295. Training.
[INFO] SpaceCleaner. Step: 4240000. Time Elapsed: 27039.176 s. Mean Reward: 0.227. Std of Reward: 4.452. Training.
[INFO] SpaceCleaner. Step: 4250000. Time Elapsed: 27115.267 s. Mean Reward: 2.315. Std of Reward: 5.405. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-4249914.onnx
[INFO] SpaceCleaner. Step: 4260000. Time Elapsed: 27188.243 s. Mean Reward: 4.529. Std of Reward: 8.169. Training.
[INFO] SpaceCleaner. Step: 4270000. Time Elapsed: 27262.438 s. Mean Reward: -0.520. Std of Reward: 1.962. Training.
[INFO] SpaceCleaner. Step: 4280000. Time Elapsed: 27340.142 s. Mean Reward: 2.858. Std of Reward: 6.583. Training.
[INFO] SpaceCleaner. Step: 4290000. Time Elapsed: 27413.157 s. Mean Reward: 1.729. Std of Reward: 2.741. Training.
[INFO] SpaceCleaner. Step: 4300000. Time Elapsed: 27488.310 s. Mean Reward: 4.569. Std of Reward: 6.903. Training.
[INFO] SpaceCleaner. Step: 4310000. Time Elapsed: 27566.309 s. Mean Reward: 3.556. Std of Reward: 6.855. Training.
[INFO] SpaceCleaner. Step: 4320000. Time Elapsed: 27638.038 s. Mean Reward: 4.628. Std of Reward: 7.796. Training.
[INFO] SpaceCleaner. Step: 4330000. Time Elapsed: 27711.972 s. Mean Reward: 3.342. Std of Reward: 4.997. Training.
[INFO] SpaceCleaner. Step: 4340000. Time Elapsed: 27785.002 s. Mean Reward: 1.730. Std of Reward: 4.617. Training.
[INFO] SpaceCleaner. Step: 4350000. Time Elapsed: 27862.698 s. Mean Reward: 3.107. Std of Reward: 4.674. Training.
[INFO] SpaceCleaner. Step: 4360000. Time Elapsed: 27936.600 s. Mean Reward: 3.793. Std of Reward: 5.704. Training.
[INFO] SpaceCleaner. Step: 4370000. Time Elapsed: 28009.912 s. Mean Reward: 1.595. Std of Reward: 6.558. Training.
[INFO] SpaceCleaner. Step: 4380000. Time Elapsed: 28088.027 s. Mean Reward: 4.582. Std of Reward: 5.224. Training.
[INFO] SpaceCleaner. Step: 4390000. Time Elapsed: 28160.853 s. Mean Reward: 0.765. Std of Reward: 3.084. Training.
[INFO] SpaceCleaner. Step: 4400000. Time Elapsed: 28234.971 s. Mean Reward: 1.371. Std of Reward: 4.436. Training.
[INFO] SpaceCleaner. Step: 4410000. Time Elapsed: 28313.718 s. Mean Reward: 4.289. Std of Reward: 6.155. Training.
[INFO] SpaceCleaner. Step: 4420000. Time Elapsed: 28386.020 s. Mean Reward: 1.631. Std of Reward: 5.217. Training.
[INFO] SpaceCleaner. Step: 4430000. Time Elapsed: 28461.072 s. Mean Reward: 0.706. Std of Reward: 3.319. Training.
[INFO] SpaceCleaner. Step: 4440000. Time Elapsed: 28537.665 s. Mean Reward: 3.637. Std of Reward: 2.688. Training.
[INFO] SpaceCleaner. Step: 4450000. Time Elapsed: 28610.965 s. Mean Reward: 2.061. Std of Reward: 3.384. Training.
[INFO] SpaceCleaner. Step: 4460000. Time Elapsed: 28685.872 s. Mean Reward: 1.155. Std of Reward: 4.105. Training.
[INFO] SpaceCleaner. Step: 4470000. Time Elapsed: 28761.998 s. Mean Reward: 3.049. Std of Reward: 5.399. Training.
[INFO] SpaceCleaner. Step: 4480000. Time Elapsed: 28836.717 s. Mean Reward: 0.886. Std of Reward: 2.682. Training.
[INFO] SpaceCleaner. Step: 4490000. Time Elapsed: 28913.455 s. Mean Reward: 3.316. Std of Reward: 5.786. Training.
[INFO] SpaceCleaner. Step: 4500000. Time Elapsed: 28987.631 s. Mean Reward: 5.683. Std of Reward: 8.238. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-4499944.onnx
[INFO] SpaceCleaner. Step: 4510000. Time Elapsed: 29063.515 s. Mean Reward: 2.846. Std of Reward: 4.882. Training.
[INFO] SpaceCleaner. Step: 4520000. Time Elapsed: 29138.864 s. Mean Reward: 2.177. Std of Reward: 3.413. Training.
[INFO] SpaceCleaner. Step: 4530000. Time Elapsed: 29213.318 s. Mean Reward: 1.731. Std of Reward: 4.239. Training.
[INFO] SpaceCleaner. Step: 4540000. Time Elapsed: 29289.852 s. Mean Reward: 1.427. Std of Reward: 3.827. Training.
[INFO] SpaceCleaner. Step: 4550000. Time Elapsed: 29364.371 s. Mean Reward: 2.433. Std of Reward: 5.250. Training.
[INFO] SpaceCleaner. Step: 4560000. Time Elapsed: 29438.308 s. Mean Reward: 4.567. Std of Reward: 5.128. Training.
[INFO] SpaceCleaner. Step: 4570000. Time Elapsed: 29516.732 s. Mean Reward: 3.327. Std of Reward: 5.634. Training.
[INFO] SpaceCleaner. Step: 4580000. Time Elapsed: 29591.755 s. Mean Reward: 5.901. Std of Reward: 8.147. Training.
[INFO] SpaceCleaner. Step: 4590000. Time Elapsed: 29669.423 s. Mean Reward: 1.408. Std of Reward: 4.516. Training.
[INFO] SpaceCleaner. Step: 4600000. Time Elapsed: 29748.640 s. Mean Reward: 3.451. Std of Reward: 6.472. Training.
[INFO] SpaceCleaner. Step: 4610000. Time Elapsed: 29824.827 s. Mean Reward: 1.270. Std of Reward: 3.618. Training.
[INFO] SpaceCleaner. Step: 4620000. Time Elapsed: 29899.070 s. Mean Reward: 4.193. Std of Reward: 5.904. Training.
[INFO] SpaceCleaner. Step: 4630000. Time Elapsed: 29979.112 s. Mean Reward: 2.637. Std of Reward: 5.126. Training.
[INFO] SpaceCleaner. Step: 4640000. Time Elapsed: 30054.913 s. Mean Reward: 2.304. Std of Reward: 5.368. Training.
[INFO] SpaceCleaner. Step: 4650000. Time Elapsed: 30131.498 s. Mean Reward: 4.670. Std of Reward: 6.340. Training.
[INFO] SpaceCleaner. Step: 4660000. Time Elapsed: 30215.354 s. Mean Reward: 3.046. Std of Reward: 7.455. Training.
[INFO] SpaceCleaner. Step: 4670000. Time Elapsed: 30286.867 s. Mean Reward: 1.864. Std of Reward: 3.948. Training.
[INFO] SpaceCleaner. Step: 4680000. Time Elapsed: 30364.035 s. Mean Reward: 2.129. Std of Reward: 5.662. Training.
[INFO] SpaceCleaner. Step: 4690000. Time Elapsed: 30440.501 s. Mean Reward: 2.755. Std of Reward: 2.239. Training.
[INFO] SpaceCleaner. Step: 4700000. Time Elapsed: 30518.623 s. Mean Reward: 4.167. Std of Reward: 7.696. Training.
[INFO] SpaceCleaner. Step: 4710000. Time Elapsed: 30594.052 s. Mean Reward: 4.242. Std of Reward: 7.699. Training.
[INFO] SpaceCleaner. Step: 4720000. Time Elapsed: 30670.669 s. Mean Reward: 0.051. Std of Reward: 2.378. Training.
[INFO] SpaceCleaner. Step: 4730000. Time Elapsed: 30749.241 s. Mean Reward: 7.533. Std of Reward: 5.040. Training.
[INFO] SpaceCleaner. Step: 4740000. Time Elapsed: 30826.656 s. Mean Reward: 3.620. Std of Reward: 7.192. Training.
[INFO] SpaceCleaner. Step: 4750000. Time Elapsed: 30901.095 s. Mean Reward: 0.074. Std of Reward: 3.123. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-4749864.onnx
[INFO] SpaceCleaner. Step: 4760000. Time Elapsed: 30980.599 s. Mean Reward: 1.236. Std of Reward: 3.494. Training.
[INFO] SpaceCleaner. Step: 4770000. Time Elapsed: 31057.091 s. Mean Reward: 3.387. Std of Reward: 4.798. Training.
[INFO] SpaceCleaner. Step: 4780000. Time Elapsed: 31134.514 s. Mean Reward: 1.209. Std of Reward: 2.313. Training.
[INFO] SpaceCleaner. Step: 4790000. Time Elapsed: 31213.589 s. Mean Reward: 6.157. Std of Reward: 6.878. Training.
[INFO] SpaceCleaner. Step: 4800000. Time Elapsed: 31288.879 s. Mean Reward: 1.543. Std of Reward: 4.060. Training.
[INFO] SpaceCleaner. Step: 4810000. Time Elapsed: 31366.792 s. Mean Reward: 0.821. Std of Reward: 3.211. Training.
[INFO] SpaceCleaner. Step: 4820000. Time Elapsed: 31445.024 s. Mean Reward: 1.346. Std of Reward: 5.240. Training.
[INFO] SpaceCleaner. Step: 4830000. Time Elapsed: 31521.812 s. Mean Reward: 2.105. Std of Reward: 4.059. Training.
[INFO] SpaceCleaner. Step: 4840000. Time Elapsed: 31598.141 s. Mean Reward: 3.463. Std of Reward: 6.036. Training.
[INFO] SpaceCleaner. Step: 4850000. Time Elapsed: 31675.141 s. Mean Reward: 2.476. Std of Reward: 4.026. Training.
[INFO] SpaceCleaner. Step: 4860000. Time Elapsed: 31754.736 s. Mean Reward: 2.212. Std of Reward: 2.419. Training.
[INFO] SpaceCleaner. Step: 4870000. Time Elapsed: 31827.475 s. Mean Reward: 1.039. Std of Reward: 2.964. Training.
[INFO] SpaceCleaner. Step: 4880000. Time Elapsed: 31902.316 s. Mean Reward: 3.072. Std of Reward: 4.079. Training.
[INFO] SpaceCleaner. Step: 4890000. Time Elapsed: 31978.739 s. Mean Reward: 1.379. Std of Reward: 3.303. Training.
[INFO] SpaceCleaner. Step: 4900000. Time Elapsed: 32054.622 s. Mean Reward: 1.448. Std of Reward: 5.048. Training.
[INFO] SpaceCleaner. Step: 4910000. Time Elapsed: 32128.812 s. Mean Reward: 3.294. Std of Reward: 5.006. Training.
[INFO] SpaceCleaner. Step: 4920000. Time Elapsed: 32206.270 s. Mean Reward: 2.708. Std of Reward: 3.854. Training.
[INFO] SpaceCleaner. Step: 4930000. Time Elapsed: 32279.477 s. Mean Reward: 1.329. Std of Reward: 2.530. Training.
[INFO] SpaceCleaner. Step: 4940000. Time Elapsed: 32353.382 s. Mean Reward: 0.315. Std of Reward: 2.593. Training.
[INFO] SpaceCleaner. Step: 4950000. Time Elapsed: 32431.327 s. Mean Reward: 2.583. Std of Reward: 3.472. Training.
[INFO] SpaceCleaner. Step: 4960000. Time Elapsed: 32505.863 s. Mean Reward: 2.264. Std of Reward: 3.404. Training.
[INFO] SpaceCleaner. Step: 4970000. Time Elapsed: 32579.507 s. Mean Reward: 1.778. Std of Reward: 4.047. Training.
[INFO] SpaceCleaner. Step: 4980000. Time Elapsed: 32657.712 s. Mean Reward: 4.258. Std of Reward: 3.897. Training.
[INFO] SpaceCleaner. Step: 4990000. Time Elapsed: 32730.831 s. Mean Reward: 2.508. Std of Reward: 4.815. Training.
[INFO] SpaceCleaner. Step: 5000000. Time Elapsed: 32806.347 s. Mean Reward: 3.868. Std of Reward: 5.573. Training.
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-4999937.onnx
[INFO] Exported results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-5000193.onnx
[INFO] Copied results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner\SpaceCleaner-5000193.onnx to results\space_cleaner_gravity_custom_ppo_v4\SpaceCleaner.onnx.
PS E:\Storage\Coding\Unity\Space Cleaning Sim2> 